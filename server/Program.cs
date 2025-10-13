#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using db;
using db.data;
using server.sfx;
using System.Text;
using System.Threading;
using db.Repositories;
using db.Services;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using RageRealm.Shared.Configuration.WebServer;

#endregion

namespace server
{
    internal class Program
    {
        private static readonly List<HttpListenerContext> currentRequests = new();
        private static HttpListener listener;

        internal static XmlDataService GameDataService { get; set; }
        internal static ILogger Logger { get; private set; }
        public static WebServerConfiguration Config { get; private set; } = new();
        public static ServiceProvider Services { get; set; }
        internal static string InstanceId { get; set; }

        public static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.Name = "Entry";
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            Config = configuration.Get<WebServerConfiguration>();

            var serviceBuilder = new ServiceCollection();
            serviceBuilder.AddSingleton(Config);

            var connString =
                $"server={Config.Database.Host};userid={Config.Database.User};password={Config.Database.Password};database={Config.Database.Name};AllowPublicKeyRetrieval=True;SslMode=none;Convert Zero Datetime=True;";
            serviceBuilder.AddDbContext<ServerDbContext>(options =>
                options.UseMySql(connString, ServerVersion.AutoDetect(connString)));

            
            // Initialize Logging Service
            serviceBuilder.AddLogging(builder =>
            {
                builder.ClearProviders();

                builder.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                    options.IncludeScopes = true;
                });
                
                if (!Enum.TryParse(Config.Logging.LogLevel, true, out LogLevel logLevel))
                {
                    logLevel = LogLevel.Information;
                }
                
                builder.SetMinimumLevel(logLevel);

                if (Config.Logging.EnableConsole)
                {
                    builder.AddConsole();
                }

                if (Config.Logging.EnableDebug)
                {
                    builder.AddDebug();
                }

                if (Config.Logging.EnableFile && !string.IsNullOrEmpty(Config.Logging.FilePath))
                {
                    builder.AddFile(Config.Logging.FilePath);
                }
            });

            // Register Unit of Work and Services
            serviceBuilder.AddScoped<IUnitOfWork, UnitOfWork>();
            serviceBuilder.AddScoped<AccountService>();

            // Register Repositories
            serviceBuilder.AddScoped<IAccountRepository, AccountRepository>();
            serviceBuilder.AddScoped<IDeathRepository, DeathRepository>();
            serviceBuilder.AddScoped<INewsRepository, NewsRepository>();
            serviceBuilder.AddScoped<ICharacterRepository, CharacterRepository>();
            serviceBuilder.AddScoped<IClientErrorRepository, ClientErrorRepository>();
            serviceBuilder.AddScoped<IUnlockedClassRepository, UnlockedClassRepository>();
            serviceBuilder.AddSingleton<XmlDataService>();
            
            Services = serviceBuilder.BuildServiceProvider();

            GameDataService = Services.GetRequiredService<XmlDataService>();
            InstanceId = Guid.NewGuid().ToString();

            Logger = Services.GetRequiredService<ILogger<Program>>();

            Console.CancelKeyPress += (sender, e) => e.Cancel = true;

            Logger.LogInformation("Starting RageRealmWebServer {Version}...", typeof(Program).Assembly.GetName().Version);
            Logger.LogInformation("Instance ID: {InstanceId}", InstanceId);
            
            var port = Config.Port;

            if (RunPreCheck(port))
            {
                listener = new HttpListener();
                listener.Prefixes.Add($"http://*:{port}/");
                listener.Start();
                Logger.LogInformation("Listening at port {Port}...", port);

                await ListenLoopAsync();
            }
            else
            {
                Logger.LogError("Port {Port} is occupied. Can't start listening...\nPress ESC to exit.", port);
                while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
            }

            Logger.LogError("Terminating...");
            while (currentRequests.Count > 0)
                await Task.Delay(50);

            listener?.Stop();
            GameDataService.Dispose();
        }

        private static bool RunPreCheck(int port)
        {
            var props = IPGlobalProperties.GetIPGlobalProperties();
            return props.GetActiveTcpConnections().All(_ => _.LocalEndPoint.Port != port)
                   && props.GetActiveTcpListeners().All(_ => _.Port != port);
        }

        private static async Task ListenLoopAsync()
        {
            try
            {
                while (listener.IsListening)
                {
                    var context = await listener.GetContextAsync();
                    _ = Task.Run(async () => await ProcessRequestAsync(context));
                }
            }
            catch (HttpListenerException ex) when (ex.ErrorCode == 995)
            {
                // Listener stopped
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in ListenLoopAsync");
            }
        }

        private static async Task ProcessRequestAsync(HttpListenerContext context)
        {
            try
            {
                if (context.Request.Url == null)
                {
                    Logger.LogWarning("Received request with null URL from: {Client}",
                        context.Request.RemoteEndPoint);
                    return;
                }

                Logger.LogInformation("Request \"{Endpoint}\" from: {Client}",
                    context.Request.Url.LocalPath, context.Request.RemoteEndPoint);

                if (context.Request.Url.LocalPath.Contains("sfx") ||
                    context.Request.Url.LocalPath.Contains("music"))
                {
                    var sfx = new Sfx();
                    await sfx.HandleRequest(context);
                    context.Response.Close();
                    return;
                }

                string s;
                if (context.Request.Url.LocalPath.IndexOf(".") == -1)
                    s = "server" + context.Request.Url.LocalPath.Replace("/", ".");
                else
                    s = "server" +
                        context.Request.Url.LocalPath.Remove(context.Request.Url.LocalPath.IndexOf("."))
                            .Replace("/", ".");

                var t = Type.GetType(s);

                if (t == null)
                {
                    await SendFileOr404Async(context);
                }
                else
                {
                    var handler = Activator.CreateInstance(t) as RequestHandler;
                    if (handler == null)
                    {
                        await using var wtr = new StreamWriter(context.Response.OutputStream);
                        await wtr.WriteAsync(
                            $"<Error>Class \"{t.FullName}\" is not of the type RequestHandler.</Error>");
                    }
                    else
                    {
                        await handler.HandleRequest(context);
                    }
                }
            }
            catch (Exception e)
            {
                currentRequests.Remove(context);
                await using var wtr = new StreamWriter(context.Response.OutputStream);
                await wtr.WriteAsync(e.ToString());
                Logger.LogError(e, "Error processing request");
            }
            finally
            {
                context.Response.Close();
            }
        }

        private static async Task SendFileOr404Async(HttpListenerContext context)
        {
            var file = "game" + (context.Request.RawUrl == "/" ? "/index.php" : context.Request.RawUrl);
            if (file.Contains("?"))
                file = file.Remove(file.IndexOf('?'));

            if (Directory.Exists(file))
            {
                var indexFile = Directory.GetFiles(file, "index.*", SearchOption.TopDirectoryOnly).FirstOrDefault();
                file = indexFile ?? file;
            }

            if (File.Exists(file))
            {
                if (file.StartsWith("game/Testing.html"))
                    file = Config.TestingOnline ? "game/Testing.html" : "game/TestingIsOffline.html";

                await SendFileAsync(file, context);
            }
            else
            {
                await SendFileAsync("game/404.html", context);
            }
        }

        private static readonly Dictionary<string, string> replaceVars = new()
        {
            { "{URL}", "PROPERTYCALL:RawUrl" },
            { "{GAMECLIENT}", "PATH:game/version.txt" },
            { "{TESTINGCLIENT}", "PATH:game/testingVersion.txt" },
            { "{TRANSFERENGINEVERSION}", account.getProdAccount.TRANSFERENGINEVERSION },
        };

        public static async Task SendFileAsync(string path, HttpListenerContext context)
        {
            var ext = new FileInfo(path).Extension.TrimStart('.');
            context.Response.ContentType = GetContentType(ext);

            byte[] buffer;

            if (context.Response.ContentType is "text/html" or "text/javascript" or "text/css")
            {
                var send = await File.ReadAllTextAsync(path);

                if (ext == "php")
                {
                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo("php\\php-cgi.exe",
                        $"-f {path} {GetPhpArgumentQuery(context.Request.QueryString)}")
                    {
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    p.Start();
                    send = await p.StandardOutput.ReadToEndAsync();
                }

                foreach (var toReplace in replaceVars)
                {
                    if (toReplace.Value.StartsWith("PATH"))
                    {
                        var tmp = await File.ReadAllTextAsync(toReplace.Value.Split(':')[1]);
                        send = send.Replace(toReplace.Key, tmp);
                    }
                    else if (toReplace.Value.StartsWith("PROPERTYCALL"))
                    {
                        var property = context.Request.GetType().GetProperty(toReplace.Value.Split(':')[1]);
                        if (property != null)
                            send = send.Replace(toReplace.Key, property.GetValue(context.Request)?.ToString());
                    }
                    else
                        send = send.Replace(toReplace.Key, toReplace.Value);
                }

                buffer = Encoding.UTF8.GetBytes(send);
            }
            else
            {
                buffer = await File.ReadAllBytesAsync(path);
            }

            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "OK";
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private static string GetPhpArgumentQuery(NameValueCollection query)
        {
            var sb = new StringBuilder();
            foreach (string q in query.Keys)
                sb.Append(q + "=" + query[q]);
            return sb.ToString();
        }

        private static string GetContentType(string fileExtention) =>
            fileExtention switch
            {
                "html" or "shtml" or "htm" or "php" => "text/html",
                "js" => "text/javascript",
                "swf" => "application/x-shockwave-flash",
                "css" => "text/css",
                "png" => "image/png",
                "gif" => "image/gif",
                _ => "text/html"
            };

        public static async Task SendEmailAsync(System.Net.Mail.MailMessage message, bool enableSsl)
        {
            try
            {
                var m = new MimeMessage();
                m.From.Add(new MailboxAddress("Forgot Password", message.From.Address));
                m.To.Add(new MailboxAddress("", message.To[0].Address));
                m.Subject = message.Subject;
                m.Body = new TextPart(message.IsBodyHtml ? "html" : "plain") { Text = message.Body };

                using var client = new SmtpClient();
                await client.ConnectAsync(Config.Smtp.Host, Config.Smtp.Port, enableSsl);
                await client.AuthenticateAsync(new NetworkCredential(Config.Smtp.Email, Config.Smtp.Password));
                await client.SendAsync(m);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, " Error sending email");
            }
        }
    }
}
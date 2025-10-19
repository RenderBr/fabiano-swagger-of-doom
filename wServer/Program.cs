using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using db.data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RageRealm.Shared.Configuration.WorldServer;
using Serilog;
using wServer;
using wServer.Events;
using wServer.networking;
using wServer.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;

internal class Program
{
    public static bool WhiteList { get; private set; }
    public static bool Verify { get; private set; }
    public static WorldServerConfiguration Config { get; private set; } = new();
    public static DateTime WhiteListTurnOff { get; private set; }
    public static IServiceProvider Services { get; set; }
    
    private static async Task Main(string[] args)
    {
        Console.Title = "RealmRage - World Server v" + Server.VERSION;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.Name = "Entry";

        // auto-create config if missing
        if (!File.Exists("appsettings.json"))
        {
            var defaultConfig = new WorldServerConfiguration();
            var json = System.Text.Json.JsonSerializer.Serialize(defaultConfig,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync("appsettings.json", json);
            Console.WriteLine(
                "Default configuration file created. Please review 'appsettings.json' and restart the server.");
            Console.ReadKey();
            return;
        }

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(cfg =>
            {
                cfg.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureLogging((ctx, logging) => { logging.ClearProviders(); })
            .UseSerilog((ctx, services, configuration) =>
            {
                var config = ctx.Configuration.Get<WorldServerConfiguration>();

                configuration.Enrich.FromLogContext();

                if (config.Logging.EnableConsole)
                {
                    configuration.WriteTo.Async(a => a.Console(
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext,-45} {Scope} {Message:lj}{NewLine}{Exception}"));
                }

                if (config.Logging.EnableFile)
                {
                    configuration.WriteTo.Async(a => a.File(
                        config.Logging.FilePath,
                        retainedFileCountLimit: 7,
                        shared: true,
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Scope} {Message:lj}{NewLine}{Exception}",
                        rollingInterval:
                        Serilog.RollingInterval.Day));
                }

                if (config.Logging.EnableDebug)
                {
                    configuration.MinimumLevel.Debug();
                }
                else
                {
                    configuration.MinimumLevel.Information();
                }
            })
            .ConfigureServices((ctx, services) =>
            {
                var config = ctx.Configuration.Get<WorldServerConfiguration>();
                services.AddSingleton(config);

                services
                    .AddSingleton<RsaService>()
                    .AddSingleton<XmlDataService>()
                    .AddDataServices(ctx.Configuration)
                    .AddRepositoryServices()
                    .AddSingleton<IEventBus, EventBus>()
                    .AddHandlingServices()
                    .AddSingleton<Server>()
                    .AddHostedService<WorldServerHostedService>();
            })
            .Build();

        Services = host.Services;
        await host.RunAsync();
    }
}
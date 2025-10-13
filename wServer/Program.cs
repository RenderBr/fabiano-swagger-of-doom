using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using db;
using db.data;
using db.Repositories;
using db.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RageRealm.Shared.Configuration;
using RageRealm.Shared.Configuration.WorldServer;
using wServer.Factories;
using wServer.networking;
using wServer.realm;

internal class Program
{
    public static bool WhiteList { get; private set; }
    public static bool Verify { get; private set; }

    public static ILogger Logger { get; private set; }
    private static RealmManager manager;

    public static WorldServerConfiguration Config { get; private set; } = new();
    public static DateTime WhiteListTurnOff { get; private set; }
    public static ServiceProvider Services { get; set; }

    private static async Task Main(string[] args)
    {
        Console.Title = "RealmRage - World Server v" + Server.VERSION;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        try
        {
            if (!File.Exists("appsettings.json"))
            {
                var defaultConfig = new WorldServerConfiguration();
                var json = System.Text.Json.JsonSerializer.Serialize(defaultConfig, new System.Text.Json.JsonSerializerOptions
                { WriteIndented = true });
                await File.WriteAllTextAsync("appsettings.json", json);
                Console.WriteLine("Default configuration file created. Please review 'appsettings.json' and restart the server.");
                Console.ReadKey();
                return;
            }
            
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = configBuilder.Build();

            Config = configuration.Get<WorldServerConfiguration>();

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.Name = "Entry";

            // setup DI
            var serviceBuilder = new ServiceCollection();
            serviceBuilder.AddSingleton(Config);

            // Initialize Logging Service
            serviceBuilder.AddLogging(builder =>
            {
                builder.ClearProviders();

                if (!Enum.TryParse(Config.Logging.LogLevel, true, out LogLevel logLevel))
                {
                    logLevel = LogLevel.Information;
                }

                builder.SetMinimumLevel(logLevel);

                if (Config.Logging.EnableConsole)
                {
                    builder.AddSimpleConsole(options =>
                    {
                        options.SingleLine = true;
                        options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                        options.IncludeScopes = true;
                        options.UseUtcTimestamp = true;
                    });
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

            var connString =
                $"server={Config.Database.Host};userid={Config.Database.User};password={Config.Database.Password};database={Config.Database.Name};SslMode=none;Convert Zero Datetime=True;";
            serviceBuilder.AddDbContext<ServerDbContext>(options =>
                options.UseMySql(connString, ServerVersion.AutoDetect(connString)));

            // register your services/repositories
            serviceBuilder.AddScoped<IUnitOfWork, UnitOfWork>();
            serviceBuilder.AddScoped<AccountService>();
            serviceBuilder.AddScoped<IAccountRepository, AccountRepository>();
            serviceBuilder.AddScoped<ICharacterRepository, CharacterRepository>();
            serviceBuilder.AddScoped<IDailyQuestRepository, DailyQuestRepository>();
            serviceBuilder.AddScoped<IDeathRepository, DeathRepository>();
            serviceBuilder.AddScoped<IGuildRepository, GuildRepository>();
            serviceBuilder.AddScoped<INewsRepository, NewsRepository>();
            serviceBuilder.AddScoped<IPetRepository, PetRepository>();
            serviceBuilder.AddScoped<IVaultRepository, VaultRepository>();
            serviceBuilder.AddScoped<IStatRepository, StatRepository>();

            serviceBuilder.AddSingleton<XmlDataService>();

            serviceBuilder.Configure<RealmConfiguration>(configuration.GetSection("Realm"));
            serviceBuilder.AddSingleton<IGameWorldFactory, GameWorldFactory>();
            serviceBuilder.AddSingleton<RealmManager>();

            Services = serviceBuilder.BuildServiceProvider();
            Logger = Services.GetRequiredService<ILogger<Program>>();

            // create the realm manager
            manager = Services.GetRequiredService<RealmManager>();
            WhiteList = Config.Realm.Whitelist;
            Verify = Config.Realm.VerifyEmail;
            WhiteListTurnOff = Config.Realm.WhitelistTurnOff;

            manager.Initialize();
            manager.RunAsync();

            // create the servers
            var server = new Server(manager);
            var policy = new PolicyServer();

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                Logger.LogInformation("Ctrl+C detected. Shutting down...");
                _ = ShutdownAsync(server, policy); // fire & forget async shutdown
            };

            await policy.StartAsync();
            await server.StartAsync();

            if (Config.Realm.BroadcastNews && File.Exists("news.txt"))
                _ = Task.Run(AutoBroadcastNews);

            Logger.LogInformation("Server initialized. Press ESC to exit...");

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
                await Task.Delay(50);

            await ShutdownAsync(server, policy);
        }
        catch (Exception e)
        {
            Logger.LogCritical(e, "Fatal error during initialization");
            if (manager != null)
            {
                foreach (var c in manager.Clients.Values)
                    c.Disconnect();
            }

            Console.ReadLine();
        }
    }

    private static async Task ShutdownAsync(Server server, PolicyServer policy)
    {
        try
        {
            Logger.LogInformation("Terminating...");
            await server.StopAsync();
            await policy.StopAsync();
            await manager.StopAsync();
            Logger.LogInformation("Server terminated.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during shutdown");
        }
    }

    private static async Task AutoBroadcastNews()
    {
        var news = await File.ReadAllLinesAsync("news.txt");
        var rand = new Random();
        var cm = new ChatManager(manager);

        while (true)
        {
            cm.News(news[rand.Next(news.Length)]);
            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }
}
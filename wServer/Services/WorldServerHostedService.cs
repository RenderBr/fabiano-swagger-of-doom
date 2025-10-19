using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using wServer.realm;
using wServer.networking;

namespace wServer.Services;

public class WorldServerHostedService(ILogger<WorldServerHostedService> logger, Server server, RealmManager manager)
    : IHostedService
{
    private PolicyServer _policy;

    public async Task StartAsync(CancellationToken token)
    {
        manager.Initialize();
        await manager.RunAsync();

        _policy = new PolicyServer();
        logger.LogInformation("Starting Policy Server...");
        await _policy.StartAsync();

        logger.LogInformation("Starting Main Server...");
        await server.StartAsync();

        if (File.Exists("news.txt"))
            _ = Task.Run(AutoBroadcastNews, token);

        logger.LogInformation("Server initialized. Press Ctrl+C to exit.");
    }

    public async Task StopAsync(CancellationToken token)
    {
        logger.LogInformation("Stopping...");
        await server.StopAsync();
        await _policy.StopAsync();
        await manager.StopAsync();
        logger.LogInformation("Shutdown complete.");
    }

    private async Task AutoBroadcastNews()
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
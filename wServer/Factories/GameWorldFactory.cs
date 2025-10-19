using Microsoft.Extensions.Logging;
using wServer;
using wServer.Events;
using wServer.realm;
using wServer.realm.worlds;

namespace wServer.Factories;

public class GameWorldFactory(
    ILoggerFactory loggerFactory,
    RealmManager realmManager,
    RealmPortalMonitor portalMonitor,
    GeneratorCache generatorCache,
    IEventBus eventBus)
    : IGameWorldFactory
{
    public GameWorld Create(int mapId, string name, bool oryxPresent)
    {
        var logger = loggerFactory.CreateLogger<World>();
        return new GameWorld(mapId, name, oryxPresent, realmManager, logger, portalMonitor, generatorCache, eventBus);
    }
}
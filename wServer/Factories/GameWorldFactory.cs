using Microsoft.Extensions.Logging;
using wServer.realm.worlds;

namespace wServer.Factories;

public class GameWorldFactory : IGameWorldFactory
{
    private readonly ILoggerFactory _loggerFactory;
    
    public GameWorldFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
    
    public GameWorld Create(int mapId, string name, bool oryxPresent)
    {
        var logger = _loggerFactory.CreateLogger<GameWorld>();
        return new GameWorld(mapId, name, oryxPresent, logger);
    }
}
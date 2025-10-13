using wServer.realm.worlds;

namespace wServer.Factories;

public interface IGameWorldFactory
{
    GameWorld Create(int mapId, string name, bool oryxPresent);
}
#region

using Microsoft.Extensions.Logging;
using wServer;
using wServer.networking;

#endregion

namespace wServer.realm.worlds
{
    public class WineCellar : World
    {
        public WineCellar(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Wine Cellar";
            ClientWorldName = "server.wine_cellar";
            Background = 0;
            AllowTeleport = false;
            Dungeon = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.winecellar.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client psr)
        {
            return Manager.AddWorld(new WineCellar(Manager, _logger, _portalMonitor, _generatorCache));
        }
    }
}
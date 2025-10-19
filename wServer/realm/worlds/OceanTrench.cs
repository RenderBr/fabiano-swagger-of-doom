#region

using Microsoft.Extensions.Logging;
using wServer;
using wServer.networking;

#endregion

namespace wServer.realm.worlds
{
    public class OceanTrench : World
    {
        public OceanTrench(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Ocean Trench";
            ClientWorldName = "server.Ocean_Trench";
            Dungeon = true;
            Background = 0;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.oceantrench.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client psr)
        {
            return Manager.AddWorld(new OceanTrench(Manager, _logger, _portalMonitor, _generatorCache));
        }
    }
}
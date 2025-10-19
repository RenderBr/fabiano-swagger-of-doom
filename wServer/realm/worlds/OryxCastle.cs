#region

using Microsoft.Extensions.Logging;
using wServer;
using wServer.networking;

#endregion

namespace wServer.realm.worlds
{
    public class OryxCastle : World
    {
        public OryxCastle(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Oryx's Castle";
            ClientWorldName = "server.Oryx's_Castle";
            Background = 0;
            AllowTeleport = false;
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.OryxCastle.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client psr)
        {
            return Manager.AddWorld(new OryxCastle(Manager, _logger, _portalMonitor, _generatorCache));
        }
    }
}
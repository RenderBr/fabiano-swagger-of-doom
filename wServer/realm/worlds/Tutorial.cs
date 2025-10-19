#region

using Microsoft.Extensions.Logging;
using wServer;
using wServer.networking;

#endregion

namespace wServer.realm.worlds
{
    public class Tutorial : World
    {
        private readonly bool isLimbo;

        public Tutorial(bool isLimbo, RealmManager manager, ILogger<World> logger,
            RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Id = TUT_ID;
            Name = "Tutorial";
            ClientWorldName = "server.tutorial";
            Background = 0;
            this.isLimbo = isLimbo;
        }

        protected override void Init()
        {
            if (!(IsLimbo = isLimbo))
            {
                LoadMap("wServer.realm.worlds.maps.tutorial.wmap", MapType.Wmap);
            }
        }

        public override World GetInstance(Client psr)
        {
            return Manager.AddWorld(new Tutorial(false, Manager, _logger, _portalMonitor, _generatorCache));
        }
    }
}
using Microsoft.Extensions.Logging;
using wServer;

namespace wServer.realm.worlds
{
    public class Kitchen : World
    {
        public Kitchen(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Kitchen";
            ClientWorldName = "server.Kitchen";
            Background = 0;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.kitchen.wmap", MapType.Wmap);
        }
    }
}
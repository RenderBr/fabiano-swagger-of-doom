using Microsoft.Extensions.Logging;
using wServer;

namespace wServer.realm.worlds
{
    public sealed class ClothBazaar : World
    {
        public ClothBazaar(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor,
            GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Id = MARKET;
            Name = "Cloth Bazaar";
            ClientWorldName = "nexus.Cloth_Bazaar";
            Background = 2;
            AllowTeleport = false;
            Difficulty = 0;
            
            Init();
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.bazzar.wmap", MapType.Wmap);
        }
    }
}
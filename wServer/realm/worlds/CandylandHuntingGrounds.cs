using Microsoft.Extensions.Logging;
using wServer;

namespace wServer.realm.worlds
{
    public class CandylandHuntingGrounds : World
    {
        public CandylandHuntingGrounds(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Candyland Hunting Grounds";
            ClientWorldName = "dungeons.Candyland_Hunting_Grounds";
            Background = 0;
            Difficulty = 3;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.candyland.wmap", MapType.Wmap);
        }
    }
}

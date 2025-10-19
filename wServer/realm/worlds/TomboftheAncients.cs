using Microsoft.Extensions.Logging;
using wServer;

namespace wServer.realm.worlds
{
    public class TomboftheAncients : World
    {
        public TomboftheAncients(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Tomb of the Ancients";
            ClientWorldName = "dungeons.Tomb_of_the_Ancients";
            Dungeon = true;
            Background = 0;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.tomb.wmap", MapType.Wmap);
        }
    }
}
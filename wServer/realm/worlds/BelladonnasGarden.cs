using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer;

namespace wServer.realm.worlds
{
    public class BelladonnasGarden : World
    {
        public BelladonnasGarden(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Belladonna's Garden";
            ClientWorldName = "dungeons.BelladonnaAPOSs_Garden";
            Background = 0;
            AllowTeleport = false;
            Difficulty = 5;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.belladonnasGarden.wmap", MapType.Wmap);
        }
    }
}

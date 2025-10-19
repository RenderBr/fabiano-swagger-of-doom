using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer;

namespace wServer.realm.worlds
{
    public class SpiderDen : World
    {
        public SpiderDen(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Spider Den";
            ClientWorldName = "dungeons.Spider_Den";
            Background = 0;
            Difficulty = 2;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.spiderden.wmap", MapType.Wmap);
        }
    }
}

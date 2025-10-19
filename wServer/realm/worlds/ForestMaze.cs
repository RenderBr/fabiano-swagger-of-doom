using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer;

namespace wServer.realm.worlds
{
    public class ForestMaze : World
    {
        public ForestMaze(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Forest Maze";
            ClientWorldName = "dungeons.Forest_Maze";
            Background = 0;
            Difficulty = 1;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.forestmaze.wmap", MapType.Wmap);
        }
    }
}

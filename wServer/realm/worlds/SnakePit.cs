using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer;
using wServer.networking;

namespace wServer.realm.worlds
{
    public class SnakePit : World
    {
        public SnakePit(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Snake Pit";
            ClientWorldName = "dungeons.Snake_Pit";
            Dungeon = true;
            Background = 0;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.snakepit.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client client)
        {
            return Manager.AddWorld(new SnakePit(Manager, _logger, _portalMonitor, _generatorCache));
        }
    }
}

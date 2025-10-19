using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer;

namespace wServer.realm.worlds
{
    public class SpriteWorld : World
    {
        public SpriteWorld(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Sprite World";
            ClientWorldName = "dungeons.Sprite_World";
            Background = 0;
            Difficulty = 2;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.spriteworld.wmap", MapType.Wmap);
        }
    }
}

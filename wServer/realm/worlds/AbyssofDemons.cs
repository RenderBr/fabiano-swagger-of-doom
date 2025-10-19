#region

using System.Threading;
using System.Threading.Tasks;
using DungeonGenerator;
using DungeonGenerator.Templates.Abyss;
using Microsoft.Extensions.Logging;
using wServer.networking;

#endregion

namespace wServer.realm.worlds
{
    public class AbyssofDemons : World
    {
        public AbyssofDemons(RealmManager manager, ILogger<World> logger,
                RealmPortalMonitor portalMonitor,GeneratorCache generatorCache)
            : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Abyss of Demons";
            ClientWorldName = "{dungeons.Abyss_of_Demons}";
            Dungeon = true;
            Background = 0;
            AllowTeleport = true;
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap(_generatorCache.NextAbyss(Seed));
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new AbyssofDemons(Manager, _logger, _portalMonitor, _generatorCache));
    }
}
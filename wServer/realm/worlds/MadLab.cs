using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer;

namespace wServer.realm.worlds
{
    public class MadLab : World
    {
        public MadLab(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Mad Lab";
            ClientWorldName = "dungeons.Mad_Lab";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = true;
        }

        public override async Task InitAsync()
        {
            await LoadMapAsync(_generatorCache.NextLab(Seed));
        }
    }
}

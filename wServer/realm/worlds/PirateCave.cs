using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer;

namespace wServer.realm.worlds
{
    public class PirateCave : World
    {
        public PirateCave(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Name = "Pirate Cave";
            ClientWorldName = "dungeons.Pirate_Cave";
            Background = 0;
            Difficulty = 1;
            AllowTeleport = true;
        }

        public override async Task InitAsync()
        {
            await LoadMapAsync(_generatorCache.NextPirateCave(Seed));
        }
    }
}

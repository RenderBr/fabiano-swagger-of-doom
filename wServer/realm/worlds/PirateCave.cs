using System.Threading.Tasks;

namespace wServer.realm.worlds
{
    public class PirateCave : World
    {
        public PirateCave(RealmManager manager) : base(manager)
        {
            Name = "Pirate Cave";
            ClientWorldName = "dungeons.Pirate_Cave";
            Background = 0;
            Difficulty = 1;
            AllowTeleport = true;
        }

        public override async Task InitAsync()
        {
            await LoadMapAsync(GeneratorCache.NextPirateCave(Seed));
        }
    }
}

using System.Threading.Tasks;

namespace wServer.realm.worlds
{
    public class MadLab : World
    {
        public MadLab(RealmManager manager) : base(manager)
        {
            Name = "Mad Lab";
            ClientWorldName = "dungeons.Mad_Lab";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = true;
        }

        public override async Task InitAsync()
        {
            await LoadMapAsync(GeneratorCache.NextLab(Seed));
        }
    }
}

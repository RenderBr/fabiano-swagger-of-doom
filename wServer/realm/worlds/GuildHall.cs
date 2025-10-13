using db;
using wServer.networking;
using Microsoft.Extensions.DependencyInjection;
using db.Repositories;

namespace wServer.realm.worlds
{
    public class GuildHall : World
    {
        public string Guild { get; set; }
        public int GuildLevel { get; set; }

        public GuildHall(string guild)
        {
            Id = GUILD_ID;
            Guild = guild;
            Name = "Guild Hall";
            Background = 0;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            // Get guild level synchronously for map loading
            int level = 0;
            using (var scope = Program.Services.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var guild = unitOfWork.Guilds.GetByNameAsync(Guild).Result; // Synchronous for simplicity
                level = guild?.Level ?? 0;
            }

            switch (level)
            {
                case 0:
                    LoadMap("wServer.realm.worlds.maps.ghall0.wmap", MapType.Wmap); break;
                case 1:
                    LoadMap("wServer.realm.worlds.maps.ghall1.wmap", MapType.Wmap); break;
                case 2:
                    LoadMap("wServer.realm.worlds.maps.ghall2.wmap", MapType.Wmap); break;
                default:
                    LoadMap("wServer.realm.worlds.maps.ghall3.wmap", MapType.Wmap); break;
            }
        }

        public override World GetInstance(Client client)
        {
            return Manager.AddWorld(new GuildHall(Guild));
        }
    }
}

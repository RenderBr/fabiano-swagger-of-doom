#region

using System;
using System.Linq;
using System.Threading.Tasks;
using db.Repositories;
using Microsoft.Extensions.DependencyInjection;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;

#endregion

namespace wServer.networking.handlers
{
    internal class ChooseNameHandler(IServiceProvider serviceProvider)
        : PacketHandlerBase<ChooseNamePacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.CHOOSENAME; }
        }

        protected override async Task HandlePacket(Client client, ChooseNamePacket packet)
        {
            string[] bannednames =
            {
                "Dick", "Fuck", "Pussy", "Cunt", "Bitch", "Nigger", "Nigga", "Niqqa", "Cunt", "Shit", "Penis", "Vagina",
                "Chent", "Niqqer", "Negro", "Ngr", "Chink", "Fag", "Faggot", "Fgt", "Fagot", "Fagit", "Ass", "Autistic",
                "Autism", "Schlong", "vag", "damn", "tits", "tlts", "retard", "asd", "Kalle", "Kaile", "Kalie",
                "xDalla", "xDalia", "xDaila", "McFarvo", "Pixl", "TheHangman", "White", "DrMini", "TEEBQNE", "TBQNEE",
                "FloFlorian", "Lore", "Dalla", "Daila", "Dalia", "Clocking", "Ciocking", "IArkani", "lArkani",
                "BunnyBomb", "Liinkii", "Gamingland", "GamingIand", "TheRegal", "TheRegaI", "ParagonX", "Cantplay",
                "Billyhendr", "Nilly", "Trapped", "Botmaker", "JustANoob", "JustANoobROTMG", "Niiiy", "niily", "niliy",
                "Lucifer", "Kithio", "Case", "Travoos", "XD", "DX", "Trol", "Troll", "lol", "lel", "OMG", "suck"
            };

            foreach (string i in bannednames)
            {
                if (i.ToLower().Equals(packet.Name) || packet.Name.ToLower().Contains(i.ToLower()))
                {
                    client.SendPacket(new NameResultPacket
                    {
                        Success = false,
                        ErrorText = "Error.nameAlreadyInUse"
                    });
                    return;
                }
            }

            using var scope = Program.Services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var account = await unitOfWork.Accounts.GetByIdAsync(long.Parse(client.Account.AccountId));
            if (account == null)
            {
                client.SendPacket(new NameResultPacket
                {
                    Success = false,
                    ErrorText = "Account not found"
                });
                return;
            }

            if (String.IsNullOrEmpty(packet.Name) || packet.Name.Length > 10)
            {
                client.SendPacket(new NameResultPacket
                {
                    Success = false,
                    ErrorText = "Error.nameIsNotAlpha"
                });
                return;
            }

            // Check if name is already taken
            var existingAccount = await unitOfWork.Accounts.FirstOrDefaultAsync(a => a.Name == packet.Name);
            if (existingAccount != null)
            {
                client.SendPacket(new NameResultPacket
                {
                    Success = false,
                    ErrorText = "Error.nameAlreadyInUse"
                });
                return;
            }

            // Check if user has enough credits (if they already chose a name)
            if (account.Credits < 1000 && account.NameChosen)
            {
                client.SendPacket(new NameResultPacket
                {
                    Success = false,
                    ErrorText = "server.not_enough_gold"
                });
                return;
            }

            try
            {
                // Update account name
                account.Name = packet.Name;
                account.NameChosen = true;

                // Deduct credits if this isn't their first name choice
                if (account.NameChosen && account.Credits >= 1000)
                {
                    account.Credits -= 1000;
                }

                await unitOfWork.SaveChangesAsync();

                // Update player state
                client.Player.Name = packet.Name;
                client.Player.NameChosen = true;
                client.Player.UpdateCount++;

                client.SendPacket(new NameResultPacket
                {
                    Success = true,
                    ErrorText = "server.buy_success"
                });
            }
            catch
            {
                client.SendPacket(new NameResultPacket
                {
                    Success = false,
                    ErrorText = "GuildChronicle.left"
                });
            }
        }
    }
}
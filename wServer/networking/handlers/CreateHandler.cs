#region

using System;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities.player;
using FailurePacket = wServer.networking.svrPackets.FailurePacket;

#endregion

namespace wServer.networking.handlers
{
    internal class CreateHandler : PacketHandlerBase<CreatePacket>
    {
        public override PacketID ID
        {
            get { return PacketID.CREATE; }
        }

        protected override async Task HandlePacket(Client client, CreatePacket packet)
        {
            await using var scope = Program.Services.CreateAsyncScope();
            var characterRepository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            int nextCharId = 1;
            if (!client.Account.IsGuestAccount)
            {
                // Get next character ID (simple approach for now)
                var existingChars = await characterRepository.GetByAccountIdAsync(long.Parse(client.Account.AccountId));
                nextCharId = existingChars.Count > 0 ? existingChars.Max(c => c.CharacterId) + 1 : 1;

                // Check max character slots
                var account = await unitOfWork.Accounts.GetByIdAsync(long.Parse(client.Account.AccountId));
                if (account == null)
                {
                    client.Disconnect();
                    return;
                }

                var liveChars =
                    await characterRepository.GetLiveCharactersByAccountIdAsync(long.Parse(client.Account.AccountId));
                if (liveChars.Count >= account.MaxCharSlot)
                {
                    client.Disconnect();
                    return;
                }
            }

            // TODO: Implement character creation logic without Database.CreateCharacter
            // client.Character = Database.CreateCharacter(client.Manager.GameData, (ushort)packet.ClassType, nextCharId);

            var ownedSkins = string.IsNullOrEmpty(client.Account.OwnedSkins)
                ? Array.Empty<int>()
                : client.Account.OwnedSkins.Split(',').Select(s => int.TryParse(s.Trim(), out var v) ? v : -1)
                    .ToArray();
            int skin = ownedSkins.Contains(packet.SkinType) ? packet.SkinType : 0;

            var newCharacter = Program.Services.GetRequiredService<CharacterCreationService>()
                .Create((ushort)packet.ClassType, nextCharId, skin);
            
            newCharacter.AccountId = long.Parse(client.Account.AccountId);
            
            try
            {
                await characterRepository.AddAsync(newCharacter);
                await unitOfWork.SaveChangesAsync();

                client.Character = Char.FromCharacter(newCharacter);
                Program.Logger.LogInformation(
                    "Created character {CharacterId} for account {AccountName}", client.Character.CharacterId, client.Account.Name);

                var target = client.Manager.Worlds[client.TargetWorld];
                var player = new Player(client.Manager, client);

                client.Player = player;

                client.SendPacket(new Create_SuccessPacket
                {
                    CharacterID = newCharacter.CharacterId,
                    ObjectID = target.EnterWorld(player)
                });
                client.Stage = ProtocalStage.Ready;
            }
            catch (Exception ex)
            {
                Program.Logger.LogError(ex,"Error creating character for account: {AccountID}", client.Account.AccountId);
                client.SendPacket(new FailurePacket
                {
                    ErrorDescription = "Failed to Load character."
                });
            }
        }
    }
}
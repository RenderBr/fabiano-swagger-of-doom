#region

using System;
using System.Threading.Tasks;
using db;
using db.Repositories;
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
    internal class LoadHandler(IServiceProvider serviceProvider) : PacketHandlerBase<LoadPacket>(serviceProvider)
    {
        public override PacketID ID => PacketID.LOAD;

        protected override async Task HandlePacket(Client client, LoadPacket packet)
        {
            using var scope = ServiceProvider.CreateScope();
            var characterRepository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();

            var character =
                await characterRepository.GetByCharacterIdAsync(long.Parse(client.Account.AccountId),
                    packet.CharacterId);
            if (character == null || character.Dead)
            {
                ServiceProvider.GetRequiredService<ILogger<LoadHandler>>().LogError(
                    "Character not found or dead. AccountId: {AccountAccountId}, CharacterId: {PacketCharacterId}",
                    client.Account.AccountId, packet.CharacterId);
                client.SendPacket(new FailurePacket
                {
                    ErrorDescription = "Character not found or dead."
                });
                client.Disconnect();
                return;
            }

            client.Character = Char.FromCharacter(character);
            var world = client.Manager.Worlds[client.TargetWorld];

            client.Player = new Player(client.Manager, client);
            var objId = world.EnterWorld(client.Player);

            client.SendPacket(new Create_SuccessPacket()
            {
                CharacterID = client.Character.CharacterId,
                ObjectID = objId
            });

            client.Stage = ProtocalStage.Ready;
        }
    }
}
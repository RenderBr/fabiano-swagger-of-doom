using System;
using System.Threading.Tasks;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm.worlds;

namespace wServer.networking.handlers
{
    internal class PetCommandHandler(IServiceProvider serviceProvider) : PacketHandlerBase<PetCommandPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.ACTIVEPETUPDATE; }
        }

        protected override Task HandlePacket(Client client, PetCommandPacket packet)
        {
            client.Manager.Logic.AddPendingAction(t =>
            {
                client.Manager.Database.DoActionAsync(db =>
                {
                    if (!(client.Player.Owner is PetYard)) return;
                    var pet = ((PetYard)client.Player.Owner).FindPetById((int)packet.PetId);

                    switch (packet.CommandId)
                    {
                        case PetCommandPacket.FOLLOW_PET:
                            if (client.Player.Pet != null) client.Player.Pet.PlayerOwner = null;
                            client.Player.Pet = pet;
                            pet.PlayerOwner = client.Player;
                            // Update character petId via repository
                            // Note: This needs proper character update implementation
                            client.SendPacket(new UpdatePetPacket
                            {
                                PetId = pet.PetId
                            });
                            client.Player.SaveToCharacter();
                            break;
                        case PetCommandPacket.UNFOLLOW_PET:
                            // Update character to unfollow pet via repository
                            client.Player.Pet.PlayerOwner = null;
                            client.Player.Pet = null;
                            client.SendPacket(new UpdatePetPacket
                            {
                                PetId = -1
                            });
                            break;
                        case PetCommandPacket.RELEASE_PET:
                            // Delete pet via repository
                            client.SendPacket(new RemovePetFromListPacket
                            {
                                PetId = pet.PetId
                            });
                            client.Player.SaveToCharacter();
                            client.Player.Owner.LeaveWorld(pet);
                            if (client.Player.Pet != null)
                                client.Player.Pet.PlayerOwner = client.Player;
                            break;
                        default:
                            client.Player.SendError("Unknown CommandId");
                            break;
                    }
                });
            });
            return Task.CompletedTask;
        }
    }
}
#region

using System.Linq;
using System.Threading.Tasks;
using RageRealm.Shared.Models;
using wServer.networking.cliPackets;
using wServer.realm;

#endregion

namespace wServer.networking.handlers
{
    internal class ReskinHandler : PacketHandlerBase<ReskinPacket>
    {
        public override PacketID ID
        {
            get { return PacketID.RESKIN; }
        }

        protected override Task HandlePacket(Client client, ReskinPacket packet)
        {
            client.Manager.Logic.AddPendingAction(t =>
            {
                if (packet.SkinId == 0)
                {
                    client.Player.PlayerSkin = 0;
                }
                else
                {
                    var owned = string.IsNullOrEmpty(client.Account.OwnedSkins)
                        ? Enumerable.Empty<int>()
                        : client.Account.OwnedSkins.Split(',').Select(s => int.TryParse(s.Trim(), out var v) ? v : -1);
                    if (owned.Contains(packet.SkinId))
                        client.Player.PlayerSkin = packet.SkinId;
                    else
                        client.Player.SendError("You do not have this skin");
                }
                client.Player.UpdateCount++;
                client.Player.SaveToCharacter();
                client.Save();
            }, PendingPriority.Networking);
            return Task.CompletedTask;
        }
    }
}
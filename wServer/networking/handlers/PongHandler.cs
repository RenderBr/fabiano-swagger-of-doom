#region

using System.Threading.Tasks;
using wServer.networking.cliPackets;

#endregion

namespace wServer.networking.handlers
{
    internal class PongHandler : PacketHandlerBase<PongPacket>
    {
        public override PacketID ID
        {
            get { return PacketID.PONG; }
        }

        protected override Task HandlePacket(Client client, PongPacket packet)
        {
            client.Player.Pong(packet.Time, packet);
            return Task.CompletedTask;
        }
    }
}
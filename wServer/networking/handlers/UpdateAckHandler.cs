#region

using System;
using System.Threading.Tasks;
using wServer.networking.cliPackets;

#endregion

namespace wServer.networking.handlers
{
    internal class UpdateAckHandler(IServiceProvider serviceProvider) : PacketHandlerBase<UpdateAckPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.UPDATEACK; }
        }

        protected override Task HandlePacket(Client client, UpdateAckPacket packet)
        {
            client.Player.UpdatesReceived++;
            return Task.CompletedTask;
        }
    }
}
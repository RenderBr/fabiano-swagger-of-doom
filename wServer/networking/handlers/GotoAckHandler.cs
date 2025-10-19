#region

using System;
using System.Threading.Tasks;
using wServer.networking.cliPackets;

#endregion

namespace wServer.networking.handlers
{
    internal class GotoAckHandler(IServiceProvider serviceProvider)
        : PacketHandlerBase<GotoAckPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.GOTOACK; }
        }

        protected override Task HandlePacket(Client client, GotoAckPacket packet)
        {
            //TODO: Implement something.
            return Task.CompletedTask;
        }
    }
}
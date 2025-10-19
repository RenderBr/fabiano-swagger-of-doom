#region

using System;
using System.Threading.Tasks;
using wServer.networking.cliPackets;

#endregion

namespace wServer.networking.handlers
{
    internal class AOEAckHandler(IServiceProvider serviceProvider)
        : PacketHandlerBase<AOEAckPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.AOEACK; }
        }

        protected override Task HandlePacket(Client client, AOEAckPacket packet)
        {
            //TODO: Implement something.
            return Task.CompletedTask;
        }
    }
}
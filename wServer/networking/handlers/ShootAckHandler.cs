#region

using System;
using System.Threading.Tasks;
using wServer.networking.cliPackets;

#endregion

namespace wServer.networking.handlers
{
    internal class ShootAckHandler(IServiceProvider serviceProvider) : PacketHandlerBase<ShootAckPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.SHOOTACK; }
        }

        protected override Task HandlePacket(Client client, ShootAckPacket packet)
        {
            //TODO: Implement something
            return Task.CompletedTask;
        }
    }
}
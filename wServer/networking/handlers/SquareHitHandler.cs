#region

using System;
using System.Threading.Tasks;
using wServer.networking.cliPackets;

#endregion

namespace wServer.networking.handlers
{
    internal class SquareHitHandler(IServiceProvider serviceProvider) : PacketHandlerBase<SquareHitPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.SQUAREHIT; }
        }

        protected override Task HandlePacket(Client client, SquareHitPacket packet)
        {
            //TODO: Implement something
            return Task.CompletedTask;
        }
    }
}
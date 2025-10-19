#region

using System;
using System.Threading.Tasks;
using wServer.networking.cliPackets;

#endregion

namespace wServer.networking.handlers
{
    internal class OtherHitHandler(IServiceProvider serviceProvider) : PacketHandlerBase<OtherHitPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.OTHERHIT; }
        }

        protected override Task HandlePacket(Client client, OtherHitPacket packet)
        {
            //TODO: Implement something
            return Task.CompletedTask;
        }
    }
}
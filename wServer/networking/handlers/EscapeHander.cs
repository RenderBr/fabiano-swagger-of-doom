#region

using System;
using System.Threading.Tasks;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;

#endregion

namespace wServer.networking.handlers
{
    internal class EscapeHander(IServiceProvider serviceProvider)
        : PacketHandlerBase<EscapePacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.ESCAPE; }
        }

        protected override Task HandlePacket(Client client, EscapePacket packet)
        {
            if (client.Player.Owner == null) return Task.CompletedTask;
            World world = client.Manager.GetWorld(client.Player.Owner.Id);
            if (world.Id == World.NEXUS_ID)
            {
                client.SendPacket(new TextPacket
                {
                    Stars = -1,
                    BubbleTime = 0,
                    Name = "",
                    Text = "server.already_nexus"
                });
                return Task.CompletedTask;
            }

            client.Reconnect(new ReconnectPacket
            {
                Host = "",
                Port = Program.Config.Realm.ServerPort,
                GameId = World.NEXUS_ID,
                Name = "nexus.Nexus",
                Key = Empty<byte>.Array,
            });
            return Task.CompletedTask;
        }
    }
}
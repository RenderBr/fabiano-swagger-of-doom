using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;

namespace wServer.networking.handlers
{
    internal class LeaveArenaHandler : PacketHandlerBase<LeaveArenaPacket>
    {
        public override PacketID ID
        {
            get { return PacketID.ARENA_DEATH; }
        }

        protected override Task HandlePacket(Client client, LeaveArenaPacket packet)
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

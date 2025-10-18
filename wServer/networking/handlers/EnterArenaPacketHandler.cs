#region

using System.Threading.Tasks;
using db;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.worlds;

#endregion

namespace wServer.networking.handlers
{
    internal class EnterArenaPacketHandler : PacketHandlerBase<EnterArenaPacket>
    {
        public override PacketID ID
        {
            get { return PacketID.ENTER_ARENA; }
        }

        protected override Task HandlePacket(Client client, EnterArenaPacket packet)
        {
              client.Manager.Database.DoActionAsync(async db =>
            {
                if (packet.Currency == 1)
                {
                    await db.UpdateFameAsync(client.Account, -500);
                    client.Player.CurrentFame = client.Account.Stats.Fame;
                    client.Player.UpdateCount++;
                }
                else
                {
                        db.UpdateCredit(client.Account, -50);
                        client.Player.Credits = client.Account.Credits;
                    client.SendPacket(new BuyResultPacket
                    {
                        Result = 0,
                        Message = "{server.buy_success}"
                    });
                    client.Player.UpdateCount++;
                }
                });
            client.Save();

            World world = client.Player.Manager.AddWorld(new Arena(client.Player.Manager));

            client.Reconnect(new ReconnectPacket
            {
                Host = "",
                Port = Program.Config.Realm.ServerPort,
                GameId = world.Id,
                Name = world.Name,
                Key = Empty<byte>.Array,
            });
            
            return Task.CompletedTask;
        }
    }
}

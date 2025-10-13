#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities.player;
using System.Threading.Tasks;
using db.JsonObjects;
using db.Models;
using RageRealm.Shared.Models;
using FailurePacket = wServer.networking.cliPackets.FailurePacket;

#endregion

namespace wServer.networking
{
    public enum ProtocalStage
    {
        Connected,
        Handshaked,
        Ready,
        Disconnected
    }

    public class Client : IDisposable
    {
        public const string SERVER_VERSION = "1.0.3.0";
        private bool disposed;

        private static readonly ILogger log = Program.Services?.GetRequiredService<ILogger<Client>>();

        public uint UpdateAckCount = 0;

        private NetworkHandler handler;

        public Client(RealmManager manager, Socket skt)
        {
            Socket = skt;
            Manager = manager;
            ReceiveKey =
                new RC4(new byte[] { 0x31, 0x1f, 0x80, 0x69, 0x14, 0x51, 0xc7, 0x1d, 0x09, 0xa1, 0x3a, 0x2a, 0x6e });
            SendKey = new RC4(new byte[]
                { 0x72, 0xc5, 0x58, 0x3c, 0xaf, 0xb6, 0x81, 0x89, 0x95, 0xcd, 0xd7, 0x4b, 0x80 });
            BeginProcess();
        }

        public RC4 ReceiveKey { get; private set; }
        public RC4 SendKey { get; private set; }
        public RealmManager Manager { get; private set; }

        public int Id { get; internal set; }

        public Socket Socket { get; internal set; }

        public Char Character { get; internal set; }

        public Account Account { get; internal set; }

        public ProtocalStage Stage { get; internal set; }

        public Player Player { get; internal set; }

        public wRandom Random { get; internal set; }
        public string ConnectedBuild { get; internal set; }
        public int TargetWorld { get; internal set; }

        public void BeginProcess()
        {
            log?.LogInformation("Received client @ {endPoint}", Socket.RemoteEndPoint);
            handler = new NetworkHandler(this, Socket);
            handler.BeginHandling();
        }

        public void SendPacket(Packet pkt)
        {
            if (Stage == ProtocalStage.Disconnected || Socket == null || !Socket.Connected)
                return;
            
            if (pkt.ID != PacketID.MOVE && pkt.ID != PacketID.NEWTICK)
            {
                log?.LogInformation("Sending packet '{packetId}' to {endPoint}", pkt.ID, Socket.RemoteEndPoint);
            }

            handler?.SendPacket(pkt);
        }

        public void SendPackets(IEnumerable<Packet> pkts)
        {
            handler?.SendPackets(pkts);
        }

        public bool IsReady()
        {
            if (Stage == ProtocalStage.Disconnected)
                return false;
            return Stage != ProtocalStage.Ready || (Player != null && (Player == null || Player.Owner != null));
        }

        internal async Task ProcessPacket(Packet pkt)
        {
            try
            {
                if (pkt.ID != PacketID.MOVE)
                {
                    log?.LogInformation("Handling packet '{packetId}'...", pkt.ID);
                }

                if (pkt.ID == (PacketID)255) return;
                IPacketHandler handler;
                if (!PacketHandlers.Handlers.TryGetValue(pkt.ID, out handler))
                    log?.LogWarning("Unhandled packet '{packetId}'", pkt.ID);
                else
                {
                    await handler.Handle(this, (ClientPacket)pkt).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                log?.LogError(e, "Error when handling packet '{packetId}'...", pkt.ID);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            try
            {
                if (Stage == ProtocalStage.Disconnected) return;
                Stage = ProtocalStage.Disconnected;

                handler?.Stop();
                
                if (Socket.Connected)
                {
                    try
                    {
                        Socket.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    }

                    Socket.Close();
                }
                
                handler?.Dispose();

                if (Account != null)
                    DisconnectFromRealm();
            }
            catch (Exception e)
            {
                log?.LogError(e, "Error during client disconnect");
            }
        }

        public Task Save()
        {
            return Manager.Database.DoActionAsync(db =>
            {
                try
                {
                    string w = null;
                    if (Player != null)
                    {
                        Player.SaveToCharacter();
                        if (Player.Owner != null)
                        {
                            if (Player.Owner.Id == -6) return;
                            w = Player.Owner.Name;
                        }
                    }

                    if (Character != null)
                    {
                        if (w != null) db.UpdateLastSeen(Account.AccountId, Character.CharacterId, w);
                        db.SaveCharacter(Account, Character);
                    }

                    db.UnlockAccount(Account);
                }
                catch (Exception ex)
                {
                    log?.LogCritical(ex, "SaveException");
                }
            });
        }

        //Following must execute, network loop will discard disconnected client, so logic loop
        private void DisconnectFromRealm()
        {
            Manager.Logic.AddPendingAction(async t =>
            {
                try
                {
                    await Save().ConfigureAwait(false);
                    await Manager.DisconnectAsync(this).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    log?.LogError(ex, "Error during disconnect from realm");
                }
            }, PendingPriority.Destruction);
        }

        public void Reconnect(ReconnectPacket pkt)
        {
            Manager.Logic.AddPendingAsyncAction(async t =>
            {
                try
                {
                    await Save().ConfigureAwait(false);
                    SendPacket(pkt);
                }
                catch (Exception ex)
                {
                    log?.LogError(ex, "Error during reconnect");
                }
            }, PendingPriority.Destruction);
        }

        public void GiftCodeReceived(string type)
        {
            //Use later
            switch (type)
            {
                case "Pong":
                    break;
                case "LevelUp":
                    break;
            }

            AddGiftCode(db.JsonObjects.GiftCode.GenerateRandom(Manager.GameDataService));
        }

        private async void AddGiftCode(db.JsonObjects.GiftCode code)
        {
            await Manager.Database.DoActionAsync(async db =>
            {
                var key = await db.GenerateGiftcodeAsync(code.ToJson(), Account.AccountId).ConfigureAwait(false);

                //var message = new MailMessage();
                //message.To.Add(Account.Email);
                //message.IsBodyHtml = true;
                //message.Subject = "You received a new GiftCode";
                //message.From = new MailAddress(Program.Settings.GetValue<string>("serverEmail", ""));
                //message.Body = "<center>Your giftcode is: " + code + "</br> Check the items in your giftcode <a href=\"" + Program.Settings.GetValue<string>("serverDomain", "localhost") + "/CheckGiftCode.html\" target=\"_blank\">here</a> or redeem the code <a href=\"" + Program.Settings.GetValue<string>("serverDomain", "localhost") + "/RedeemGiftCode.html\" target=\"_blank\">here</a></center>";

                //Program.SendEmail(message);
                Player.SendInfo(
                    $"You have received a new GiftCode: {key}\nRedeem it at: {Program.Config.Realm.ServerDomain}/GiftCode.html or\n type /giftcode to scan it with your mobile via qr code");
            }).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (disposed) return;
            handler?.Dispose();
            handler = null;
            ReceiveKey = null;
            SendKey = null;
            Manager = null;
            Socket = null;
            Character = null;
            Account = null;
            Player?.Dispose();
            Player = null;
            Random = null;
            ConnectedBuild = null;
            disposed = true;
        }
    }
}
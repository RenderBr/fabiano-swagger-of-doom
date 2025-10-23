#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities.player;
using System.Threading.Tasks;
using db.Models;
using RageRealm.Shared.Models;
using wServer.Factories;

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
        private bool disposed;

        private ILogger log;

        public uint UpdateAckCount = 0;

        private NetworkHandler handler;
        IServiceProvider _serviceProvider;
        INetworkHandlerFactory _networkHandlerFactory;

        public Client(IServiceProvider serviceProvider, Socket skt)
        {
            _serviceProvider = serviceProvider;
            log = serviceProvider.GetRequiredService<ILogger<Client>>();
            Manager = serviceProvider.GetRequiredService<RealmManager>();
            _networkHandlerFactory = serviceProvider.GetRequiredService<INetworkHandlerFactory>();
            Socket = skt;
            ReceiveKey =
                new RC4([0x31, 0x1f, 0x80, 0x69, 0x14, 0x51, 0xc7, 0x1d, 0x09, 0xa1, 0x3a, 0x2a, 0x6e]);
            SendKey = new RC4([0x72, 0xc5, 0x58, 0x3c, 0xaf, 0xb6, 0x81, 0x89, 0x95, 0xcd, 0xd7, 0x4b, 0x80]);
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
        public bool Reconnecting { get; set; } = false;

        public static PacketID[] ExcludePacketsFromLogging =
        [
            PacketID.NEWTICK,
            PacketID.UPDATE,
            PacketID.PING,
            PacketID.MOVE,
            PacketID.UPDATEACK,
            PacketID.SHOOTACK,
            PacketID.ENEMYSHOOT,
            PacketID.PLAYERSHOOT,
            PacketID.ALLYSHOOT,
            PacketID.OTHERHIT,
            PacketID.PLAYERHIT,
            PacketID.PONG
        ];

        public void BeginProcess()
        {
            log?.LogInformation("Received client @ {endPoint}", Socket.RemoteEndPoint);
            handler = _networkHandlerFactory.CreateNetworkHandler(_serviceProvider, this, Socket);
            handler.BeginHandling();
        }

        public void SendPacket(Packet pkt)
        {
            if (Stage == ProtocalStage.Disconnected || Socket == null || !Socket.Connected)
                return;

            if (!ExcludePacketsFromLogging.Contains(pkt.ID))
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
                if (!ExcludePacketsFromLogging.Contains(pkt.ID))
                {
                    log?.LogInformation("Handling packet '{packetId}'...", pkt.ID);
                }

                if (pkt.ID == (PacketID)255) return;

                var packetHandlers = _serviceProvider.GetRequiredService<PacketHandlers>();
                IPacketHandler handler;
                if (!packetHandlers.Handlers.TryGetValue(pkt.ID, out handler))
                    log?.LogWarning("Unhandled packet '{packetId}'", pkt.ID);
                else
                {
                    await handler.Handle(this, (ClientPacket)pkt).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                log?.LogError(e, "Error when handling packet '{packetId}'...", pkt.ID);
            }
        }

        public void DisconnectFromRealm()
        {
            if (Stage == ProtocalStage.Disconnected) return;
            Stage = ProtocalStage.Disconnected;

            Manager.Logic.AddPendingAsyncAction(async t =>
            {
                try
                {
                    await Save().ConfigureAwait(false);
                    await Manager.DisconnectAsync(this).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    log?.LogError(ex, "Error during soft disconnect from realm");
                }
            }, PendingPriority.Destruction);
        }

        public void Disconnect()
        {
            if (Stage == ProtocalStage.Disconnected) return;
            Stage = ProtocalStage.Disconnected;

            handler?.Stop();

            // initiate async realm disconnect and wait for completion BEFORE disposing
            Manager.Logic.AddPendingAsyncAction(async t =>
            {
                try
                {
                    await Save().ConfigureAwait(false);
                    await Manager.DisconnectAsync(this).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    log?.LogError(ex, "Error during hard disconnect from realm");
                }
                finally
                {
                    // now that logic thread is done using us, we can clean up
                    try
                    {
                        if (Socket?.Connected == true)
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

                        Dispose();
                    }
                    catch (Exception ex)
                    {
                        log?.LogError(ex, "Error while disposing after disconnect");
                    }
                }
            }, PendingPriority.Destruction);
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

        public void Reconnect(ReconnectPacket pkt)
        {
            Reconnecting = true;

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
            if (Reconnecting) return;
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
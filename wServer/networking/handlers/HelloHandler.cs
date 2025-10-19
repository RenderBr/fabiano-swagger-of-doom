#region

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using db;
using db.Models;
using db.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.worlds;
using FailurePacket = wServer.networking.svrPackets.FailurePacket;

#endregion

namespace wServer.networking.handlers
{
    internal class HelloHandler(IServiceProvider serviceProvider) : PacketHandlerBase<HelloPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.HELLO; }
        }

        protected override async Task HandlePacket(Client client, HelloPacket packet)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<HelloHandler>>();
            if (Server.VERSION != packet.BuildVersion)
            {
                logger.LogWarning(
                    "HELLO mismatch check: client version '{PacketBuildVersion}', server version '{ServerVersion}'",
                    packet.BuildVersion, Server.VERSION);
                client.SendPacket(new FailurePacket
                {
                    ErrorId = 0,
                    ErrorDescription = "server.update_client"
                });
                client.SendPacket(new FailurePacket
                {
                    ErrorId = 4,
                    ErrorDescription = Server.VERSION
                });
                client.Disconnect();
                return;
            }

            Account account = null;
            try
            {
                await using var scope = Program.Services.CreateAsyncScope();
                var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
                account = await accountService.VerifyAsync(packet.GUID, packet.Password);
            }
            catch
            {
                // Verification failed
            }

            if (account == null)
            {
                logger.LogInformation(@"Account not verified.");
                // TODO: Create guest account using AccountService
                client.Account = new Account { Guest = true, Uuid = packet.GUID, Name = "Guest" };

                if (client.Account == null)
                {
                    logger.LogInformation(@"Account is null!");
                    client.SendPacket(new FailurePacket
                    {
                        ErrorDescription = "Invalid account."
                    });
                    logger.LogInformation("Kicking {AccountName} because account is null.",
                        client.Account.Name);
                    client.Disconnect();
                    return;
                }
            }
            else
            {
                client.Account = account;
            }

            if (!client.Account.IsGuestAccount)
            {
                int? timeout = null;

                if (DateTime.Now <= Program.WhiteListTurnOff)
                {
                    if (!IsWhiteListed(client.Account.Rank))
                    {
                        client.SendPacket(new FailurePacket
                        {
                            ErrorId = 0,
                            ErrorDescription = "You are not whitelisted!"
                        });
                        logger.LogInformation("Kicking {AccountName} because they are not whitelisted.",
                            client.Account.Name);
                        client.Disconnect();
                        return;
                    }
                }
                // TODO: Check account in use via AccountService
                //if (db.CheckAccountInUse(client.Account, ref timeout))
                //{
                //    if (timeout == null)
                //    {
                //        client.SendPacket(new FailurePacket
                //        {
                //            ErrorId = 0,
                //            ErrorDescription = "Account in use."
                //        });
                //    }
                //    else
                //    {
                //        client.SendPacket(new FailurePacket
                //        {
                //            ErrorId = 0,
                //            ErrorDescription = "Account in use. (" + timeout + " seconds until timeout.)"
                //        });
                //    }
                //    client.Disconnect();
                //    return;
                //}
            }

            logger.LogInformation(@"Client trying to connect!");
            client.ConnectedBuild = packet.BuildVersion;
            if (!client.Manager.TryConnect(client))
            {
                client.Account = null;
                client.SendPacket(new FailurePacket
                {
                    ErrorDescription = "Failed to connect."
                });
                client.Disconnect();
                logger.LogWarning(@"Failed to connect.");
            }
            else
            {
                logger.LogInformation(@"Client loading world");
                if (packet.GameId == World.NEXUS_LIMBO) packet.GameId = World.NEXUS_ID;
                World world = client.Manager.GetWorld(packet.GameId);
                if (world == null && packet.GameId == World.TUT_ID)
                    world = client.Manager.AddWorld(new Tutorial(false, client.Manager, client.Manager.WorldLogger, client.Manager.PortalMonitor, client.Manager.GeneratorCache));
                if (world == null)
                {
                    client.SendPacket(new FailurePacket
                    {
                        ErrorId = 1,
                        ErrorDescription = "Invalid world."
                    });
                    client.Disconnect();
                    return;
                }

                if (world.NeedsPortalKey)
                {
                    if (!world.PortalKey.SequenceEqual(packet.Key))
                    {
                        client.SendPacket(new FailurePacket
                        {
                            ErrorId = 1,
                            ErrorDescription = "Invalid Portal Key"
                        });
                        client.Disconnect();
                        return;
                    }

                    if (world.PortalKeyExpired)
                    {
                        client.SendPacket(new FailurePacket
                        {
                            ErrorId = 1,
                            ErrorDescription = "Portal key expired."
                        });
                        client.Disconnect();
                        return;
                    }
                }

                client.Reconnecting = false;
                logger.LogInformation(@"Client joined world {WorldID}", world.Id);
                if (packet.MapInfo.Length > 0) //Test World
                    (world as Test).LoadJson(Encoding.Default.GetString(packet.MapInfo));

                if (world.IsLimbo)
                    world = world.GetInstance(client);
                client.Random = new wRandom(world.Seed);
                client.TargetWorld = world.Id;
                client.SendPacket(new MapInfoPacket
                {
                    Width = world.Map.Width,
                    Height = world.Map.Height,
                    Name = world.Name,
                    Seed = world.Seed,
                    RealmName = world.ClientWorldName,
                    DisplayName = world.Name,
                    Difficulty = world.Difficulty,
                    Background = world.Background,
                    AllowTeleport = world.AllowTeleport,
                    ShowDisplays = world.ShowDisplays,
                    MaxPlayers = (short)world.MaxPlayers,
                    ConnectionGuid = Guid.NewGuid().ToString(),
                    GameOpenedTime = (uint)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
                    ClientXML = world.ClientXml,
                    ExtraXML = client.Manager.GameDataService.AdditionXml
                });
                client.Stage = ProtocalStage.Handshaked;
            }
        }

        private bool IsWhiteListed(int rank)
        {
            if (Program.WhiteList)
            {
                if (rank > 0) return true;
                return false;
            }

            return true;
        }
    }
}
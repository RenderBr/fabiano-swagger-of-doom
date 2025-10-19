using System;
using System.Threading.Tasks;
using wServer.networking.cliPackets;
using wServer.realm;
using wServer.networking.svrPackets;
using wServer.realm.entities.player;

namespace wServer.networking.handlers
{
    class CreateGuildHandler(IServiceProvider serviceProvider)
        : PacketHandlerBase<CreateGuildPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.CREATEGUILD; }
        }

        protected override Task HandlePacket(Client client, CreateGuildPacket packet)
        {
            client.Manager.Logic.AddPendingAction(t => Handle(client, packet));
            return Task.CompletedTask;
        }

        void Handle(Client client, CreateGuildPacket packet)
        {
            try
            {
                client.Manager.Database.DoActionAsync(async db =>
                {
                    Player player = client.Player;
                    var name = packet.Name.ToString();
                    if (player.Client.Account.Stats.Fame >= 1000)
                    {
                        if (name != "")
                        {
                            if (await db.GetGuild(name).ConfigureAwait(false) != null)
                            {
                                player.Client.SendPacket(new CreateGuildResultPacket()
                                {
                                    Success = false,
                                    ErrorText =
                                        "{\"key\":\"server.create_guild_error\",\"tokens\":{\"error\":\"Guild already exists.\"}}"
                                });
                                return;
                            }

                            try
                            {
                                if (player.Client.Account.Guild.Name == "")
                                {
                                    if (packet.Name != "")
                                    {
                                        var g = await db.CreateGuild(packet.Name, player.Client.Account)
                                            .ConfigureAwait(false);
                                        var legacyGuild = new Guild
                                            { Id = g.Id, Name = g.Name, Rank = 0, Fame = g.Fame };
                                        player.Client.Account.GuildId = g.Id;
                                        player.Client.Account.GuildRank = 0;
                                        player.Guild = GuildManager.Add(player, legacyGuild);
                                        player.Client.SendPacket(new CreateGuildResultPacket()
                                        {
                                            Success = true,
                                            ErrorText = "{\"key\":\"server.buy_success\"}"
                                        });
                                        player.CurrentFame = player.Client.Account.Stats.Fame;
                                        player.UpdateCount++;
                                        return;
                                    }
                                    else
                                    {
                                        player.Client.SendPacket(new CreateGuildResultPacket()
                                        {
                                            Success = false,
                                            ErrorText =
                                                "{\"key\":\"server.create_guild_error\",\"tokens\":{\"error\":\"Guild name cannot be blank.\"}}"
                                        });
                                        return;
                                    }
                                }
                                else
                                {
                                    player.Client.SendPacket(new CreateGuildResultPacket()
                                    {
                                        Success = false,
                                        ErrorText =
                                            "{\"key\":\"server.create_guild_error\",\"tokens\":{\"error\":\"You cannot create a guild as a guild member.\"}}"
                                    });
                                    return;
                                }
                            }
                            catch (Exception e)
                            {
                                player.Client.SendPacket(new CreateGuildResultPacket()
                                {
                                    Success = false,
                                    ErrorText = "{\"key\":\"server.create_guild_error\",\"tokens\":{\"error\":\"" +
                                                e.Message + "\"}}"
                                });
                                return;
                            }
                        }
                        else
                        {
                            player.Client.SendPacket(new CreateGuildResultPacket()
                            {
                                Success = false,
                                ErrorText =
                                    "{\"key\":\"server.create_guild_error\",\"tokens\":{\"error\":\"Guild name cannot be blank.\"}}"
                            });
                        }
                    }
                    else
                    {
                        player.Client.SendPacket(new CreateGuildResultPacket()
                        {
                            Success = false,
                            ErrorText = "{\"key\":\"server.not_enough_fame\"}"
                        });
                    }
                });
            }
            catch (Exception e)
            {
                client.SendPacket(new CreateGuildResultPacket()
                {
                    Success = false,
                    ErrorText = "{\"key\":\"server.create_guild_error\",\"tokens\":{\"error\":\"" + e.Message + "\"}}"
                });
            }
        }
    }
}
#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using db.Repositories;
using Microsoft.Extensions.DependencyInjection;
using RageRealm.Shared.Models;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm.commands;
using wServer.realm.entities;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm.commands
{
    internal class GuildChatCommand : Command
    {
        public GuildChatCommand() : base("guild")
        {
        }

        protected override Task<bool> Process(Player player, RealmTime time, string[] args)
        {
            if (!player.Guild.IsDefault)
            {
                try
                {
                    var saytext = string.Join(" ", args);

                    if (String.IsNullOrWhiteSpace(saytext))
                    {
                        player.SendHelp("Usage: /guild <text>");
                        return Task.FromResult(false);
                    }
                    else
                    {
                        player.Guild.Chat(player, saytext.ToSafeText());
                        return Task.FromResult(true);
                    }
                }
                catch
                {
                    player.SendInfo("Cannot guild chat!");
                    return Task.FromResult(false);
                }
            }
            else
                player.SendInfo("You need to be in a guild to use guild chat!");

            return Task.FromResult(false);
        }
    }

    class GChatCommand : Command
    {
        public GChatCommand() : base("g")
        {
        }

        protected override Task<bool> Process(Player player, RealmTime time, string[] args)
        {
            if (!player.Guild.IsDefault)
            {
                try
                {
                    var saytext = string.Join(" ", args);

                    if (String.IsNullOrWhiteSpace(saytext))
                    {
                        player.SendHelp("Usage: /g <text>");
                        return Task.FromResult(false);
                    }
                    else
                    {
                        player.Guild.Chat(player, saytext.ToSafeText());
                        return Task.FromResult(true);
                    }
                }
                catch
                {
                    player.SendInfo("Cannot guild chat!");
                    return Task.FromResult(false);
                }
            }
            else
                player.SendInfo("You need to be in a guild to use guild chat!");

            return Task.FromResult(false);
        }
    }

    class GuildInviteCommand : Command
    {
        public GuildInviteCommand() : base("invite")
        {
        }

        protected override Task<bool> Process(Player player, RealmTime time, string[] args)
        {
            if (String.IsNullOrWhiteSpace(args[0]))
            {
                player.SendInfo("Usage: /invite <player name>");
                return Task.FromResult(false);
            }

            if (player.Guild[player.AccountId].Rank >= 20)
            {
                foreach (var i in player.Owner.Players.Values)
                {
                    Player target = player.Owner.GetPlayerByName(args[0]);

                    if (target == null)
                    {
                        player.SendInfoWithTokens("server.invite_notfound", new KeyValuePair<string, object>[1]
                        {
                            new KeyValuePair<string, object>("player", args[0])
                        });
                        return Task.FromResult(false);
                    }

                    if (!target.NameChosen || player.Dist(target) > 20)
                    {
                        player.SendInfoWithTokens("server.invite_notfound", new KeyValuePair<string, object>[1]
                        {
                            new KeyValuePair<string, object>("player", args[0])
                        });
                        return Task.FromResult(false);
                    }

                    if (target.Guild.IsDefault)
                    {
                        target.Client.SendPacket(new InvitedToGuildPacket()
                        {
                            Name = player.Name,
                            GuildName = player.Guild[player.AccountId].Name
                        });
                        target.Invited = true;
                        player.SendInfoWithTokens("server.invite_succeed", new KeyValuePair<string, object>[2]
                        {
                            new KeyValuePair<string, object>("player", args[0]),
                            new KeyValuePair<string, object>("guild", player.Guild[player.AccountId].Name)
                        });
                        return Task.FromResult(true);
                    }
                    else
                    {
                        player.SendError("Player is already in a guild!");
                        return Task.FromResult(false);
                    }
                }
            }
            else
            {
                player.Client.SendPacket(new TextPacket()
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = "Members and initiates cannot invite!"
                });
            }

            return Task.FromResult(false);
        }
    }

    class GuildJoinCommand : Command
    {
        public GuildJoinCommand() : base("join")
        {
        }

        protected override async Task<bool> Process(Player player, RealmTime time, string[] args)
        {
            if (String.IsNullOrWhiteSpace(args[0]))
            {
                player.SendInfo("Usage: /join <guild name>");
                return false;
            }

            if (!player.Invited)
            {
                player.SendInfoWithTokens("server.guild_not_invited", new KeyValuePair<string, object>[1]
                {
                    new KeyValuePair<string, object>("guild", args[0])
                });
                return false;
            }

            using var scope = Program.Services.CreateScope();
            var guildRepository = scope.ServiceProvider.GetRequiredService<IGuildRepository>();

            var gStruct = await guildRepository.GetByNameAsync(args[0]);
            if (player.Invited == false)
            {
                player.SendInfo("You need to be invited to join a guild!");
            }

            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
            var account = await accountRepository.GetByIdAsync(player.Client.Account.AccountId);
            if (account == null)
            {
                player.SendError("Account not found.");
                return false;
            }

            if (gStruct != null)
            {
                account.Guild = gStruct;
                player.Invited = false;

                player.Client.Account.Guild = gStruct;
                GuildManager.CurrentManagers[args[0]].JoinGuild(player);
                await accountRepository.SaveChangesAsync();
            }
            else
            {
                player.SendInfoWithTokens("server.guild_join_fail", new KeyValuePair<string, object>[1]
                {
                    new KeyValuePair<string, object>("error", "Guild does not exist")
                });
            }

            return true;
        }
    }
}
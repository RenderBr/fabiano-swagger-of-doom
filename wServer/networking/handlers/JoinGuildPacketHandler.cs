using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using wServer.networking.cliPackets;
using wServer.realm;
using Microsoft.Extensions.Logging;

namespace wServer.networking.handlers
{
    class JoinGuildPacketHandler(IServiceProvider serviceProvider)
        : PacketHandlerBase<JoinGuildPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.JOINGUILD; }
        }

        protected override Task HandlePacket(Client client, JoinGuildPacket packet)
        {
            client.Manager.Logic.AddPendingAction(t => Handle(client, packet));
            return Task.CompletedTask;
        }

        void Handle(Client client, JoinGuildPacket packet)
        {
            if (!client.Player.Invited)
            {
                client.Player.SendInfoWithTokens("server.guild_not_invited", new KeyValuePair<string, object>[1]
                {
                    new KeyValuePair<string, object>("guild", packet.GuildName)
                });
                return;
            }

            client.Manager.Database.DoActionAsync(async db =>
            {
                var gStruct = await db.GetGuild(packet.GuildName).ConfigureAwait(false);
                if (client.Player.Invited == false)
                {
                    client.Player.SendInfo("You need to be invited to join a guild!");
                }

                if (gStruct != null)
                {
                    var g = await db.ChangeGuildAsync(client.Account, gStruct.Id, 0, 0, false).ConfigureAwait(false);
                    if (g != null)
                    {
                        client.Account.Guild = g;
                        GuildManager.CurrentManagers[packet.GuildName].JoinGuild(client.Player);
                    }
                }
                else
                {
                    client.Player.SendInfoWithTokens("server.guild_join_fail", new KeyValuePair<string, object>[1]
                    {
                        new KeyValuePair<string, object>("error", "Guild does not exist")
                    });
                }
            });
        }
    }
}
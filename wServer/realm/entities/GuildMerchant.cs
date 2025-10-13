#region

using db;
using System;
using wServer.networking.svrPackets;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm.entities
{
    public class GuildMerchant : SellableObject
    {
        public const int UP1 = 0x0736;
        public const int UP1C = 10000;
        public const int UP2 = 0x0737;
        public const int UP2C = 100000;
        public const int UP3 = 0x0738;
        public const int UP3C = 250000;

        public GuildMerchant(RealmManager manager, ushort objType)
            : base(manager, objType)
        {
            RankReq = 0;
            Currency = CurrencyType.GuildFame;
            switch (objType)
            {
                case UP1:
                    Price = UP1C;
                    break;
                case UP2:
                    Price = UP2C;
                    break;
                case UP3:
                    Price = UP3C;
                    break;
            }
        }

        public override void Buy(Player player)
        {
            if (!player.Guild.IsDefault)
            {
                if (player.Guild[player.AccountId].Rank >= 30)
                {
                    player.Manager.Database.DoActionAsync(async db =>
                    {
                        var guild = await db.GetGuild(0); // Placeholder - need proper guild lookup
                        if (guild != null && guild.GuildFame >= Price)
                        {
                           // Update guild level and fame via repository
                           guild.Level++;
                           guild.GuildFame -= Price;
                           // Save guild changes
                            {
                                player.Client.SendPacket(new BuyResultPacket
                                {
                                    Message = "{\"key\":\"server.sale_succeeds\"}",
                                    Result = -1
                                });
                                player.SendInfo("Please leave the Guild Hall, we need some minutes to update the Guild Hall.");
                                player.Guild.UpdateGuildHall();
                            }
                        }
                        else
                        {
                            player.SendHelp("FUCK");
                            player.Client.SendPacket(new BuyResultPacket
                            {
                                Message = "{\"key\":\"server.not_enough_fame\"}",
                                Result = 9
                            });
                        }
                        });
                }
                else
                {
                    player.Client.SendPacket(new BuyResultPacket
                    {
                        Message = "Founder or Leader rank required.",
                        Result = 0
                    });
                }
            }
        }
    }
}
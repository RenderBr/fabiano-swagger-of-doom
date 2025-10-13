using db;
using db.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using Microsoft.Extensions.DependencyInjection;
using db.Repositories;
using db.Models;

namespace wServer.networking.handlers
{
    internal class TinkerQuestHandler : PacketHandlerBase<TinkerQuestPacket>
    {
        public override PacketID ID
        {
            get { return PacketID.QUEST_REDEEM; }
        }

        protected override async Task HandlePacket(Client client, TinkerQuestPacket packet)
        {
            using (var scope = Program.Services.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                if (packet.Object.ObjectType == client.Player.Inventory[packet.Object.SlotId].ObjectType &&
                    (int)client.Player.Inventory[packet.Object.SlotId].ObjectType == Utils.FromString(client.Player.DailyQuest.Goal))
                {
                    client.SendPacket(new QuestRedeemResponsePacket
                    {
                        Success = true,
                        Message = client.Player.GetLanguageString("server.quest_complete")
                    });
                    client.Player.Inventory[packet.Object.SlotId] = null;
                    await GiveRewardsAsync(unitOfWork, client.Account, client.Player.DailyQuest.Tier - 1);
                    int tier = client.Player.DailyQuest.Tier == DailyQuestConstants.QuestsPerDay ? -1 : (client.Player.DailyQuest.Tier + 1);
                    // Update daily quest tier
                    var dailyQuest = await unitOfWork.DailyQuests.GetByAccountIdAsync(client.Account.Id);
                    if (dailyQuest != null)
                    {
                        dailyQuest.Tier = (byte)tier;
                        await unitOfWork.SaveChangesAsync();
                    }
                    // Refresh daily quest
                    client.Player.DailyQuest = await GetDailyQuestAsync(unitOfWork, client.Account.Id, Manager.GameDataService);
                    client.Player.UpdateCount++;
                    client.Player.SaveToCharacter();
                }
            }
        }

        private async Task GiveRewardsAsync(IUnitOfWork unitOfWork, Account account, int index)
        {
            switch (DailyQuestConstants.Rewards[index])
            {
                case "FortuneToken":
                    account.FortuneTokens += 2;
                    await unitOfWork.SaveChangesAsync();
                    break;
            }
        }

        private async Task<QuestItem> GetDailyQuestAsync(IUnitOfWork unitOfWork, long accountId, XmlDataService gameDataService)
        {
            var dailyQuest = await unitOfWork.DailyQuests.GetByAccountIdAsync(accountId);
            if (dailyQuest == null)
            {
                dailyQuest = new DailyQuest { AccountId = accountId };
                await unitOfWork.DailyQuests.AddAsync(dailyQuest);
                await unitOfWork.SaveChangesAsync();
            }
            // Create QuestItem from DailyQuest
            return new QuestItem
            {
                Tier = dailyQuest.Tier,
                Goal = dailyQuest.Goals, // Assuming Goals is the goal string
                Description = "Complete the daily quest", // Placeholder
                Image = "dailyQuest.png", // Placeholder
                Id = (int)accountId,
                Time = dailyQuest.Time
            };
        }
    }
}

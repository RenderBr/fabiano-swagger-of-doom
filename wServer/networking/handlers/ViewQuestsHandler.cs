using System;
using System.Threading.Tasks;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using Microsoft.Extensions.DependencyInjection;
using db.Repositories;
using db.Models;
using Microsoft.Extensions.Logging;

namespace wServer.networking.handlers
{
    internal class ViewQuestsHandler(IServiceProvider serviceProvider) : PacketHandlerBase<ViewQuestsPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.QUEST_FETCH_ASK; }
        }

        protected override async Task HandlePacket(Client client, ViewQuestsPacket packet)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var dailyQuest = await unitOfWork.DailyQuests.GetByAccountIdAsync(client.Account.Id);
                if (dailyQuest == null)
                {
                    dailyQuest = new DailyQuest { AccountId = client.Account.Id };
                    await unitOfWork.DailyQuests.AddAsync(dailyQuest);
                    await unitOfWork.SaveChangesAsync();
                }

                client.Player.DailyQuest = new QuestItem
                {
                    Tier = dailyQuest.Tier,
                    Goal = dailyQuest.Goals,
                    Description = "Complete the daily quest",
                    Image = "dailyQuest.png"
                };

                if (client.Player.DailyQuest.Tier == -1)
                {
                    // All quests completed
                    client.Player.SendInfo("No available quests found.");
                }

                client.SendPacket(new QuestFetchResponsePacket
                {
                    Quests = [],
                    NextRefreshPieces = 1
                });
                //1 token image: http://rotmg.kabamcdn.com/DailyQuest1FortuneToken.png
                //2 token image: http://rotmg.kabamcdn.com/DailyQuest2FortuneToken.png
            }
        }
    }
}
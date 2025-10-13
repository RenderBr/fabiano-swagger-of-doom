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
    internal class ViewQuestsHandler : PacketHandlerBase<ViewQuestsPacket>
    {
        public override PacketID ID
        {
            get { return PacketID.QUEST_FETCH_ASK; }
        }

        protected override async Task HandlePacket(Client client, ViewQuestsPacket packet)
        {
            using (var scope = Program.Services.CreateScope())
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
                    client.SendPacket(AllQuestsCompleted());
                    client.Player.SendInfo("No available quests found.");
                    return;
                }

                client.SendPacket(new QuestFetchResponsePacket
                {
                    Description = client.Player.DailyQuest.Description,
                    Goal = client.Player.DailyQuest.Goal,
                    Tier = client.Player.DailyQuest.Tier,
                    Image = client.Player.DailyQuest.Image
                });
                //1 token image: http://rotmg.kabamcdn.com/DailyQuest1FortuneToken.png
                //2 token image: http://rotmg.kabamcdn.com/DailyQuest2FortuneToken.png
            }
        }

        public QuestFetchResponsePacket AllQuestsCompleted()
        {
            return new QuestFetchResponsePacket
            {
                Description = "",
                Goal = "",
                Tier = -1,
                Image = ""
            };
        }
    }
}

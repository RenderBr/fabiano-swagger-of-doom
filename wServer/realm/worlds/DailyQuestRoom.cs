#region

using System.Collections.Generic;
using System.Threading.Tasks;
using wServer.realm.entities;
using wServer.realm.entities.player;
using db;
using db.Models;
using Microsoft.Extensions.DependencyInjection;
using db.Repositories;

#endregion

namespace wServer.realm.worlds
{
    public class DailyQuestRoom : World
    {
        public DailyQuestRoom()
        {
            Name = "Daily Quest Room";
            ClientWorldName = "{nexus.Daily_Quest_Room}";
            Background = 0;
            AllowTeleport = false;
            Difficulty = -1;
        }

        protected override async Task InitAsync()
        {
            await LoadMapAsync("wServer.realm.worlds.maps.dailyQuest.wmap", MapType.Wmap);
        }

        public override int EnterWorld(Entity entity)
        {
            int ret = base.EnterWorld(entity);
            if (entity is Player)
            {
                Timers.Add(new WorldTimer(2000, async (w, t) =>
                {
                    using (var scope = Program.Services.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var dailyQuest = await unitOfWork.DailyQuests.GetByAccountIdAsync(long.Parse((entity as Player).AccountId));
                        QuestItem q;
                        if (dailyQuest == null)
                        {
                            dailyQuest = new DailyQuest { AccountId = long.Parse((entity as Player).AccountId) };
                            await unitOfWork.DailyQuests.AddAsync(dailyQuest);
                            await unitOfWork.SaveChangesAsync();
                            q = new QuestItem { Tier = 1, Goal = "Default", Description = "Complete quest", Image = "quest.png" };
                        }
                        else
                        {
                            q = new QuestItem
                            {
                                Tier = dailyQuest.Tier,
                                Goal = dailyQuest.Goals,
                                Description = "Complete the daily quest",
                                Image = "dailyQuest.png"
                            };
                        }
                        (entity as Player).Client.SendPacket(new networking.svrPackets.QuestFetchResponsePacket
                        {
                            Tier = q.Tier,
                            Image = q.Image,
                            Goal = q.Goal,
                            Description = q.Description
                        });
                    }
                }));
            }
            return ret;
        }
    }
}
#region

using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using RageRealm.Shared.Models;
using wServer;
using wServer.realm.entities.player;
using wServer.realm.terrain;

#endregion

namespace wServer.realm.worlds
{
    public class Test : World
    {
        public string js = null;

        public Test(RealmManager manager, ILogger<World> logger, RealmPortalMonitor portalMonitor, GeneratorCache generatorCache) : base(manager, logger, portalMonitor, generatorCache)
        {
            Id = TEST_ID;
            Name = "Test";
            Background = 0;
        }

        public void LoadJson(string json)
        {
            js = json;
            LoadMap(json);
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            foreach (KeyValuePair<int, Player> i in Players)
            {
                if (i.Value.Client.Account.Rank < 2)
                {
                    i.Value.Client.Disconnect();
                }
            }
        }

        protected override void Init() { }
    }
}
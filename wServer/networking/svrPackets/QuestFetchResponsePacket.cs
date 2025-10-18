using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wServer.networking.svrPackets
{
    public class QuestFetchResponsePacket : ServerPacket
    {
        public QuestResponseItem[] Quests { get; set; }
        public short NextRefreshPieces { get; set; }


        public override PacketID ID
        {
        	get { return PacketID.QUEST_FETCH_RESPONSE; }
        }
        
        public override Packet CreateInstance()
        {
            return new QuestFetchResponsePacket();
        }
        
        protected override void Read(Client client, NReader rdr)
        {
            // server doesn't read this packet
        }
        
        protected override void Write(Client client, NWriter wtr)
        {
            wtr.Write((short)Quests.Length);
            foreach (var quest in Quests)
                quest.Write(client, wtr);
            wtr.Write(NextRefreshPieces);
        }
    }

    public class QuestResponseItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Expiration { get; set; }
        public int[] Requirements { get; set; }
        public int[] Rewards { get; set; }
        public bool Completed { get; set; }
        public bool ItemOfChoice { get; set; }
        public bool Repeatable { get; set; }
        public int Category { get; set; }
        public int Weight { get; set; }
        
        public void Write(Client psr, NWriter wtr)
        {
            wtr.WriteUTF(Id);
            wtr.WriteUTF(Name);
            wtr.WriteUTF(Description);
            wtr.WriteUTF(Expiration);
            wtr.Write(Requirements.Length);
            foreach (var req in Requirements)
                wtr.Write(req);
            wtr.Write(Rewards.Length);
            foreach (var rew in Rewards)
                wtr.Write(rew);
            wtr.Write(Completed);
            wtr.Write(ItemOfChoice);
            wtr.Write(Repeatable);
            wtr.Write(Category);
            wtr.Write(Weight);
        }
    }
}

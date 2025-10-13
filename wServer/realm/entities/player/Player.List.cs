#region

using db;
using System.Collections.Generic;
using wServer.networking.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    partial class Player
    {
        public void SendAccountList(List<string> list, int id)
        {
            for (var i = 0; i < list.Count; i++)
                list[i] = list[i].Trim();

            Client.SendPacket(new AccountListPacket
            {
                AccountListId = id,
                AccountIds = list.ToArray(),
                LockAction = -1
            });
        }

        public bool IsUserInLegends()
        {
            //Week
                // Check legends via repository
                // Note: This needs proper death/legend checking implementation
                // For now returning false as placeholder
                return false;

            //Month
            //All Time
                // Check month and all-time legends via repository
                // Note: This needs proper death/legend checking implementation

            return false;
        }
    }
}

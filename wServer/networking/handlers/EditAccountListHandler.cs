#region

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db;
using RageRealm.Shared.Models;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities.player;

#endregion

namespace wServer.networking.handlers
{
    internal class EditAccountListHandler : PacketHandlerBase<EditAccountListPacket>
    {
        public override PacketID ID
        {
            get { return PacketID.EDITACCOUNTLIST; }
        }

        protected override Task HandlePacket(Client client, EditAccountListPacket packet)
        {
            Player target;
            if (client.Player.Owner == null) return Task.CompletedTask;
            client.Manager.Logic.AddPendingAction(t =>
            {
                 client.Manager.Database.DoActionAsync(async db =>
                {
                    target = client.Player.Owner.GetEntity(packet.ObjectId) is Player ? client.Player.Owner.GetEntity(packet.ObjectId) as Player : null;
                    if (target == null) return;
                    if(client.Account.AccountId == target.AccountId)
                    {
                        SendFailure("You cannot do that with yourself.");
                        return;
                    }
                    switch (packet.AccountListId)
                    {
                        case AccountListPacket.LOCKED_LIST_ID:
                            if (packet.Add)
                            {
                                // TODO: implement via repository; append to CSV for now
                                client.Account.Locked = string.Join(",", (client.Player.Locked ?? new List<string>()).Concat(new[] { target.AccountId }));
                                client.Player.Locked.Add(target.AccountId);
                            }
                            else
                            {
                                var list = client.Player.Locked ?? new List<string>();
                                list.Remove(target.AccountId);
                                client.Account.Locked = string.Join(",", list);
                                client.Player.Locked.Remove(target.AccountId);
                            }
                            break;
                        case AccountListPacket.IGNORED_LIST_ID:
                            if (packet.Add)
                            {
                                client.Account.Ignored = string.Join(",", (client.Player.Ignored ?? new List<string>()).Concat(new[] { target.AccountId }));
                                client.Player.Ignored.Add(target.AccountId);
                            }
                            else
                            {
                                var ilist = client.Player.Ignored ?? new List<string>();
                                ilist.Remove(target.AccountId);
                                client.Account.Ignored = string.Join(",", ilist);
                                client.Player.Ignored.Remove(target.AccountId);
                            }
                            break;
                    }
                            
                    //List<string> list;
                    //if (packet.AccountListId == LOCKED_LIST_ID)
                    //    list = client.Player.Locked;
                    //else if (packet.AccountListId == IGNORED_LIST_ID)
                    //    list = client.Player.Ignored;
                    //else return;
                    //if (list == null)
                    //    list = new List<string>();
                    //Player player = client.Player.Owner.GetEntity(packet.ObjectId) as Player;
                    //if (player == null) return;
                    //int accId = client.Account.AccountId;
                    //if (packet.Add && list.Count < 6)
                    //    list.Add(accId.ToString());
                    //else
                    //    list.Remove(accId.ToString());

                    //if (packet.Add)
                    //{
                    //    list.Add(accId.ToString());
                    //    if (packet.AccountListId == LOCKED_LIST_ID)
                    //        db.AddLock(client.Account.AccountId, accId);
                    //    if (packet.AccountListId == IGNORED_LIST_ID)
                    //        db.AddIgnore(client.Account.AccountId, accId);
                    //}
                    //else
                    //{
                    //    list.Remove(accId.ToString());
                    //    if (packet.AccountListId == LOCKED_LIST_ID)
                    //        db.RemoveLock(client.Account.AccountId, accId);
                    //    if (packet.AccountListId == IGNORED_LIST_ID)
                    //        db.RemoveIgnore(client.Account.AccountId, accId);
                    //}

                    //client.Player.SendAccountList(list, packet.AccountListId);
                         });
            }, PendingPriority.Networking);
            return Task.CompletedTask;
        }
    }
}
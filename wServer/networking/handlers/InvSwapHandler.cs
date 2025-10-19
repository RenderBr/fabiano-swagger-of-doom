#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RageRealm.Shared.Models;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.worlds;
// Alias to resolve ambiguity between db.Models.Vault and wServer.realm.worlds.Vault
using GameVault = wServer.realm.worlds.Vault;

#endregion

namespace wServer.networking.handlers
{
    internal class InvSwapHandler(IServiceProvider serviceProvider)
        : PacketHandlerBase<InvSwapPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.INVSWAP; }
        }

        protected override Task HandlePacket(Client client, InvSwapPacket packet)
        {
            var log = Program.Services.GetRequiredService<ILogger<InvSwapHandler>>();
            if (client.Player.Owner == null) return Task.CompletedTask;

            if (client.Player.Owner is PetYard)
            {
                client.SendPacket(new InvResultPacket
                {
                    Result = 0
                });
            }

            client.Manager.Logic.AddPendingAction(t =>
            {
                Entity en1 = client.Player.Owner.GetEntity(packet.SlotObject1.ObjectId);
                Entity en2 = client.Player.Owner.GetEntity(packet.SlotObject2.ObjectId);
                IContainer con1 = en1 as IContainer;
                IContainer con2 = en2 as IContainer;

                if (packet.SlotObject1.SlotId == 254 || packet.SlotObject1.SlotId == 255 ||
                    packet.SlotObject2.SlotId == 254 || packet.SlotObject2.SlotId == 255)
                {
                    if (packet.SlotObject2.SlotId == 254)
                        if (client.Player.HealthPotions < 6)
                        {
                            client.Player.HealthPotions++;
                            con1.Inventory[packet.SlotObject1.SlotId] = null;
                        }

                    if (packet.SlotObject2.SlotId == 255)
                        if (client.Player.MagicPotions < 6)
                        {
                            client.Player.MagicPotions++;
                            con1.Inventory[packet.SlotObject1.SlotId] = null;
                        }

                    if (packet.SlotObject1.SlotId == 254)
                        if (client.Player.HealthPotions > 0)
                        {
                            client.Player.HealthPotions--;
                            con2.Inventory[packet.SlotObject2.SlotId] = null;
                        }

                    if (packet.SlotObject1.SlotId == 255)
                        if (client.Player.MagicPotions > 0)
                        {
                            client.Player.MagicPotions--;
                            con2.Inventory[packet.SlotObject1.SlotId] = null;
                        }

                    if (en1 is Player)
                        (en1 as Player).Client.SendPacket(new InvResultPacket { Result = 0 });
                    else if (en2 is Player)
                        (en2 as Player).Client.SendPacket(new InvResultPacket { Result = 0 });
                    return;
                }

                //TODO: locker
                Item item1 = con1.Inventory[packet.SlotObject1.SlotId];
                Item item2 = con2.Inventory[packet.SlotObject2.SlotId];
                List<ushort> publicbags = new List<ushort>
                {
                    0x0500,
                    0x0506,
                    0x0501
                };

                if (en1.Dist(en2) > 1)
                {
                    if (en1 is Player)
                        (en1 as Player).Client.SendPacket(new InvResultPacket
                        {
                            Result = -1
                        });
                    else if (en2 is Player)
                        (en2 as Player).Client.SendPacket(new InvResultPacket
                        {
                            Result = -1
                        });
                    en1.UpdateCount++;
                    en2.UpdateCount++;
                    return;
                }

                if (!IsValid(item1, item2, con1, con2, packet, client, log))
                {
                    client.Disconnect();
                    return;
                }

                if (con2 is OneWayContainer)
                {
                    con1.Inventory[packet.SlotObject1.SlotId] = null;
                    con2.Inventory[packet.SlotObject2.SlotId] = null;
                    client.Player.DropBag(item1);
                    en1.UpdateCount++;
                    en2.UpdateCount++;
                    return;
                }

                if (con1 is OneWayContainer)
                {
                    if (con2.Inventory[packet.SlotObject2.SlotId] != null)
                    {
                        (en2 as Player)?.Client.SendPacket(new InvResultPacket { Result = -1 });
                        (con1 as OneWayContainer).UpdateCount++;
                        en2.UpdateCount++;
                        return;
                    }

                    // Note: DatabaseAdapter is used for legacy compatibility but not needed here
                    try
                    {
                        // Remove from gifts string if present
                        var gifts = string.IsNullOrEmpty(client.Account.Gifts)
                            ? new int[0]
                            : Utils.FromCommaSepString32(client.Account.Gifts);
                        var list = gifts.ToList();
                        list.Remove(con1.Inventory[packet.SlotObject1.SlotId].ObjectType);
                        client.Account.Gifts = string.Join(",", list);
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex, "Error removing gift from string.");
                    }

                    con1.Inventory[packet.SlotObject1.SlotId] = null;
                    con2.Inventory[packet.SlotObject2.SlotId] = item1;
                    (en2 as Player).CalcBoost();
                    client.Player.SaveToCharacter();
                    client.Save();
                    en1.UpdateCount++;
                    en2.UpdateCount++;
                    (en2 as Player).Client.SendPacket(new InvResultPacket { Result = 0 });
                    return;
                }

                if (en1 is Player && en2 is Player & en1.Id != en2.Id)
                {
                    client.Manager.Chat.Announce(
                        $"{en1.Name} just tried to steal items from {en2.Name}'s inventory, GTFO YOU GOD DAMN FEGIT!!!!11111oneoneoneeleven");
                    return;
                }

                ;

                con1.Inventory[packet.SlotObject1.SlotId] = item2;
                con2.Inventory[packet.SlotObject2.SlotId] = item1;

                if (item2 != null)
                {
                    if (publicbags.Contains(en1.ObjectType) && item2.Soulbound)
                    {
                        client.Player.DropBag(item2);
                        con1.Inventory[packet.SlotObject1.SlotId] = null;
                    }
                }

                if (item1 != null)
                {
                    if (publicbags.Contains(en2.ObjectType) && item1.Soulbound)
                    {
                        client.Player.DropBag(item1);
                        con2.Inventory[packet.SlotObject2.SlotId] = null;
                    }
                }

                en1.UpdateCount++;
                en2.UpdateCount++;

                if (en1 is Player)
                {
                    if (en1.Owner.Name == "Vault")
                        (en1 as Player).Client.Save();
                    (en1 as Player).CalcBoost();
                    (en1 as Player).Client.SendPacket(new InvResultPacket { Result = 0 });
                }

                if (en2 is Player)
                {
                    if (en2.Owner.Name == "Vault")
                        (en2 as Player).Client.Save();
                    (en2 as Player).CalcBoost();
                    (en2 as Player).Client.SendPacket(new InvResultPacket { Result = 0 });
                }

                if (client.Player.Owner is GameVault)
                    if ((client.Player.Owner as GameVault).PlayerOwnerName == client.Account.Name)
                        return;

                client.Player.SaveToCharacter();
                client.Save();
            }, PendingPriority.Networking);
            return Task.CompletedTask;
        }

        private bool IsValid(Item item1, Item item2, IContainer con1, IContainer con2, InvSwapPacket packet,
            Client client, ILogger log)
        {
            if (con2 is Container || con2 is OneWayContainer)
                return true;

            bool ret = false;

            if (con1 is OneWayContainer || con1 is Container)
            {
                ret = con2.AuditItem(item1, packet.SlotObject2.SlotId);

                if (!ret)
                {
                    log.LogCritical(
                        "Cheat engine detected for player {PlayerName},\nInvalid InvSwap. {Type} instead of {Type2}",
                        client.Player.Name,
                        client.Manager.GameDataService.Items[packet.SlotObject1.ObjectType].ObjectId, item1.ObjectId);
                    foreach (Player player in client.Player.Owner.Players.Values)
                        if (player.Client.Account.Rank >= 2)
                            player.SendInfo(String.Format(
                                "Cheat engine detected for player {0},\nInvalid InvSwap. {1} instead of {2}",
                                client.Player.Name,
                                client.Manager.GameDataService.Items[packet.SlotObject1.ObjectType].ObjectId,
                                item1.ObjectId));
                }
            }

            if (con1 is Player && con2 is Player)
            {
                ret = con1.AuditItem(item1, packet.SlotObject2.SlotId) &&
                      con2.AuditItem(item2, packet.SlotObject1.SlotId);

                if (!ret)
                {
                    log.LogCritical(
                        "Cheat engine detected for player {PlayerName},\nInvalid InvSwap. {Type} instead of {Type2}",
                        client.Player.Name, item1.ObjectId,
                        client.Manager.GameDataService.Items[packet.SlotObject2.ObjectType].ObjectId);
                    foreach (Player player in client.Player.Owner.Players.Values)
                        if (player.Client.Account.Rank >= 2)
                            player.SendInfo(String.Format(
                                "Cheat engine detected for player {0},\nInvalid InvSwap. {1} instead of {2}",
                                client.Player.Name, item1.ObjectId,
                                client.Manager.GameDataService.Items[packet.SlotObject2.ObjectType].ObjectId));
                }
            }

            return ret;
        }
    }
}
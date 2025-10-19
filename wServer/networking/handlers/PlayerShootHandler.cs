#region

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RageRealm.Shared.Models;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;

#endregion

namespace wServer.networking.handlers
{
    internal class PlayerShootPacketHandler(IServiceProvider serviceProvider) : PacketHandlerBase<PlayerShootPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.PLAYERSHOOT; }
        }

        protected override Task HandlePacket(Client client, PlayerShootPacket packet)
        {
            client.Manager.Logic.AddPendingAction(t =>
            {
                if (client.Player.Owner == null) return;

                Item item = client.Player.Manager.GameDataService.Items[(ushort)packet.ContainerType];
                int stype = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (client.Player.Inventory[i]?.ObjectType == packet.ContainerType)
                    {
                        stype = i;
                        break;
                    }
                }

                if (client.Player.SlotTypes[stype] != item.SlotType && client.Account.Rank < 2)
                {
                    ServiceProvider.GetRequiredService<ILogger<PlayerShootPacketHandler>>().LogCritical(
                        "{PlayerName} is trying to cheat (Weapon doesnt match the slot type)", client.Player.Name);
                    client.Player.SendError(
                        "This cheating attempt has beed logged and a message was send to all online admins.");
                    client.Disconnect();
                    foreach (Player player in client.Player.Owner.Players.Values)
                        if (player.Client.Account.Rank >= 2)
                            player.SendInfo(String.Format(
                                "Player {0} is shooting with a weapon that doesnt match the class slot type.",
                                client.Player.Name));
                    return;
                }

                ProjectileDesc prjDesc = item.Projectiles[0]; //Assume only one
                Projectile prj = client.Player.PlayerShootProjectile(
                    packet.BulletId, prjDesc, item.ObjectType,
                    packet.Time, packet.Position, packet.Angle);
                client.Player.Owner.EnterWorld(prj);
                client.Player.BroadcastSync(new AllyShootPacket
                {
                    OwnerId = client.Player.Id,
                    Angle = packet.Angle,
                    ContainerType = packet.ContainerType,
                    BulletId = packet.BulletId,
                }, p => p != client.Player && client.Player.Dist(p) < 25);
                client.Player.FameCounter.Shoot(prj);
            }, PendingPriority.Networking);
            return Task.CompletedTask;
        }
    }
}
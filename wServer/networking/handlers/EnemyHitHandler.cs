#region

using System;
using System.Threading.Tasks;
using RageRealm.Shared.Models;
using wServer.networking.cliPackets;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.networking.handlers
{
    internal class EnemyHitHandler(IServiceProvider serviceProvider)
        : PacketHandlerBase<EnemyHitPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.ENEMYHIT; }
        }

        protected override Task HandlePacket(Client client, EnemyHitPacket packet)
        {
            if (client.Player.Owner == null) return Task.CompletedTask;

            client.Manager.Logic.AddPendingAction(t =>
            {
                Entity entity = client.Player.Owner.GetEntity(packet.TargetId);
                if (entity != null) //Tolerance
                {
                    Projectile prj = (client.Player as IProjectileOwner).Projectiles[packet.BulletId];
                    if (prj != null)
                        prj.ForceHit(entity, t);
                }
            }, PendingPriority.Networking);

            return Task.CompletedTask;
        }
    }
}
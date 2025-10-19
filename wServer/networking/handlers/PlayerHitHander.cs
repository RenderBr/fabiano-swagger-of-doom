#region

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using wServer.networking.cliPackets;
using wServer.realm.entities;

#endregion

namespace wServer.networking.handlers
{
    internal class PlayerHitHander(IServiceProvider serviceProvider) : PacketHandlerBase<PlayerHitPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.PLAYERHIT; }
        }

        protected override Task HandlePacket(Client client, PlayerHitPacket packet)
        {
            client.Manager.Logic.AddPendingAction(t => Handle(client, packet));
            return Task.CompletedTask;
        }

        private void Handle(Client client, PlayerHitPacket packet)
        {
            var log = ServiceProvider.GetRequiredService<ILogger<PlayerHitHander>>();
            try
            {
                if (client.Player.Owner != null)
                {
                    Projectile proj;
                    if (
                        client.Player.Owner.Projectiles.TryGetValue(
                            new Tuple<int, byte>(packet.ObjectId, packet.BulletId), out proj))
                    {
                        foreach (ConditionEffect effect in proj.Descriptor.Effects)
                        {
                            if (effect.Target == 1)
                            {
                                if (client.Player.Pet != null)
                                    client.Player.Pet.ApplyConditionEffect(effect);
                            }
                            else
                                client.Player.ApplyConditionEffect(effect);
                        }

                        client.Player.Damage(proj.Damage, proj.ProjectileOwner.Self);
                    }
                    else
                        log.LogError("Can't register playerhit: {ObjectId} - {BulletId}", packet.ObjectId,
                            packet.BulletId);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error in PlayerHit");
            }
        }
    }
}
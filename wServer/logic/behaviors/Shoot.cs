#region

using System;
using RageRealm.Shared.Models;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities;

#endregion

namespace wServer.logic.behaviors
{
    public class Shoot : CycleBehavior
    {
        //State storage: cooldown timer

        protected readonly double angleOffset;
        protected readonly int coolDownOffset;
        protected readonly int count;
        protected readonly double predictive;
        protected readonly int projectileIndex;
        protected readonly double radius;
        protected readonly double shootAngle;
        protected double? fixedAngle;
        protected Cooldown coolDown;
        protected double? defaultAngle;

        public Shoot(double radius, int count = 1, double? shootAngle = null,
            int projectileIndex = 0, double? fixedAngle = null,
            double angleOffset = 0, double? defaultAngle = null,
            double predictive = 0, int coolDownOffset = 0,
            Cooldown coolDown = new Cooldown())
        {
            this.radius = radius;
            this.count = count;
            this.shootAngle = count == 1 ? 0 : (shootAngle ?? 360.0 / count) * Math.PI / 180;
            this.fixedAngle = fixedAngle * Math.PI / 180;
            this.angleOffset = angleOffset * Math.PI / 180;
            this.defaultAngle = defaultAngle * Math.PI / 180;
            this.projectileIndex = projectileIndex;
            this.predictive = predictive;
            this.coolDownOffset = coolDownOffset;
            this.coolDown = coolDown.Normalize();
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            // convert initial ms cooldown offset to ticks (ensure positive offsets survive rounding)
            if (coolDownOffset <= 0)
            {
                state = 0;
            }
            else
            {
                var ticks = (int)Math.Round(coolDownOffset / (float)host.Manager.Logic.MsPT);
                state = ticks < 1 ? 1 : ticks;
            }
        }


        private static double Predict(Entity host, Entity target, ProjectileDesc desc)
        {
            Position? history = target.TryGetHistory(100);
            if (history == null)
                return 0;

            double originalAngle = Math.Atan2(history.Value.Y - host.Y, history.Value.X - host.X);
            double newAngle = Math.Atan2(target.Y - host.Y, target.X - host.X);


            float bulletSpeed = desc.Speed / 100;
            double angularVelo = (newAngle - originalAngle) / (100 / 1000f);
            return angularVelo * bulletSpeed;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            if (state == null) return;
            int cool = (int)state;
            Status = CycleStatus.NotStarted;

            if (cool <= 0)
            {
                if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                    return;

                Entity player = host.GetNearestEntity(radius, null);
                if (player != null || defaultAngle != null || fixedAngle != null)
                {
                    ProjectileDesc desc = host.ObjectDesc.Projectiles[projectileIndex];
                    double a = fixedAngle ?? (player == null
                        ? defaultAngle.Value
                        : Math.Atan2(player.Y - host.Y, player.X - host.X));

                    a += angleOffset;
                    if (predictive != 0 && player != null)
                        a += Predict(host, player, desc) * predictive;

                    int dmg = (host as Character)?.Random.Next(desc.MinDamage, desc.MaxDamage)
                              ?? Random.Next(desc.MinDamage, desc.MaxDamage);

                    double startAngle = a - shootAngle * (count - 1) / 2;
                    byte prjId = 0;
                    Position prjPos = new Position { X = host.X, Y = host.Y };
                    for (int i = 0; i < count; i++)
                    {
                        Projectile prj = host.CreateProjectile(
                            desc, host.ObjectType, dmg, time.tickTimes, prjPos,
                            (float)(startAngle + shootAngle * i));
                        host.Owner.EnterWorld(prj);
                        if (i == 0) prjId = prj.ProjectileId;
                    }

                    host.Owner.BroadcastPacket(new ShootPacket
                    {
                        BulletId = prjId,
                        OwnerId = host.Id,
                        Position = prjPos,
                        Angle = (float)startAngle,
                        Damage = (short)dmg,
                        BulletType = (byte)desc.BulletType,
                        AngleInc = (float)shootAngle,
                        NumShots = (byte)count
                    }, null);
                }

                // convert new cooldown (ms) → ticks
                cool = coolDown.NextTicks(Random, host.Manager.Logic.MsPT);
                Status = CycleStatus.Completed;
            }
            else
            {
                cool -= 1; // one tick passed
                Status = CycleStatus.InProgress;
            }

            state = cool;
        }
    }
}
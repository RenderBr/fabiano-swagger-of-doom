#region

using System;
using Mono.Game;
using RageRealm.Shared.Models;
using wServer.realm;

#endregion

namespace wServer.logic.behaviors
{
    public class Wander : CycleBehavior
    {
        //State storage: direction & remain time


        private static Cooldown period = new Cooldown(500, 200);
        private readonly float speed;

        public Wander(double speed)
        {
            this.speed = (float) speed;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            var storage = state as WanderStorage ?? new WanderStorage();

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
            {
                Status = CycleStatus.NotStarted;
                return;
            }

            if (storage.RemainingDistance <= 0)
            {
                Vector2 direction;
                do
                {
                    direction = new Vector2(Random.Next(-1, 2), Random.Next(-1, 2));
                } while (direction.LengthSquared() < float.Epsilon);

                direction.Normalize();
                storage.Direction = direction;

                // distance to travel before picking a new direction
                var travelSeconds = period.Next(Random) / 1000f;
                storage.RemainingDistance = MathF.Max(0.1f, host.GetSpeed(speed) * travelSeconds);
                Status = CycleStatus.Completed;
            }
            else
            {
                Status = CycleStatus.InProgress;
            }

            float dist = host.GetSpeed(speed) * (time.thisTickTimes / 1000f);
            if (host.ValidateAndMove(host.X + storage.Direction.X * dist, host.Y + storage.Direction.Y * dist))
                host.UpdateCount++;

            storage.RemainingDistance -= dist;
            state = storage;
        }


        private class WanderStorage
        {
            public Vector2 Direction;
            public float RemainingDistance;
        }
    }
}
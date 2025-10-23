#region

using System;
using Mono.Game;
using RageRealm.Shared.Models;
using wServer.realm;
using wServer.realm.entities.player;

#endregion

namespace wServer.logic.behaviors
{
    public class Follow : CycleBehavior
    {
        //State storage: follow state
        private readonly float acquireRange;
        private readonly int duration;
        private readonly float range;
        private readonly float speed;
        private Cooldown coolDown;

        public Follow(double speed, double acquireRange = 10, double range = 6,
            int duration = 0, Cooldown coolDown = new Cooldown())
        {
            this.speed = (float)speed;
            this.acquireRange = (float)acquireRange;
            this.range = (float)range;
            this.duration = duration;
            this.coolDown = coolDown.Normalize(duration == 0 ? 0 : 1000);
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            var s = state as FollowState ?? new FollowState();

            if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed))
            {
                Status = CycleStatus.NotStarted;
                return;
            }

            var player = host.GetNearestEntity(acquireRange, null) as Player;
            int ToTicks(int ms)
            {
                if (ms <= 0)
                    return 0;

                var ticks = (int)Math.Round(ms / (float)host.Manager.Logic.MsPT);
                return ticks < 1 ? 1 : ticks;
            }

            Status = CycleStatus.NotStarted;

            switch (s.State)
            {
                case F.DontKnowWhere:
                    if (player != null && s.RemainingTime <= 0)
                    {
                        s.State = F.Acquired;
                        s.RemainingTime = ToTicks(duration);
                        Status = CycleStatus.InProgress;
                    }
                    else if (s.RemainingTime > 0)
                        s.RemainingTime--;

                    break;

                case F.Acquired:
                    if (player == null)
                    {
                        s.State = F.DontKnowWhere;
                        s.RemainingTime = 0;
                        Status = CycleStatus.Completed;
                        break;
                    }

                    if (s.RemainingTime <= 0 && duration > 0)
                    {
                        s.State = F.DontKnowWhere;
                        s.RemainingTime = ToTicks(coolDown.Next(Random));
                        Status = CycleStatus.Completed;
                        break;
                    }

                    if (s.RemainingTime > 0)
                        s.RemainingTime--;

                    var vect = new Vector2(player.X - host.X, player.Y - host.Y);
                    if (vect.Length > range)
                    {
                        vect.X -= Random.Next(-2, 2) / 2f;
                        vect.Y -= Random.Next(-2, 2) / 2f;
                        vect.Normalize();

                        var dist = host.GetSpeed(speed) * (time.thisTickTimes / 1000f);
                        if (host.ValidateAndMove(host.X + vect.X * dist, host.Y + vect.Y * dist))
                            host.UpdateCount++;

                        Status = CycleStatus.InProgress;
                    }
                    else
                    {
                        s.State = F.Resting;
                        s.RemainingTime = 0;
                        Status = CycleStatus.Completed;
                    }

                    break;

                case F.Resting:
                    if (player == null)
                    {
                        s.State = F.DontKnowWhere;
                        s.RemainingTime = ToTicks(duration);
                        Status = CycleStatus.NotStarted;
                        break;
                    }

                    var delta = new Vector2(player.X - host.X, player.Y - host.Y);
                    if (delta.Length > range + 1)
                    {
                        s.State = F.Acquired;
                        s.RemainingTime = ToTicks(duration);
                        Status = CycleStatus.InProgress;
                    }
                    else
                    {
                        Status = CycleStatus.Completed;
                    }

                    break;
            }

            state = s;
        }

        private enum F
        {
            DontKnowWhere,
            Acquired,
            Resting
        }

        private class FollowState
        {
            public int RemainingTime;
            public F State;
        }
    }
}
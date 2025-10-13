#region

using System;
using System.Threading.Tasks;
using RageRealm.Shared.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace wServer.realm
{
    public class WorldTimer
    {
        private readonly ILogger<WorldTimer> _logger;
        private readonly Action<World, RealmTime> cb;
        private readonly Func<World, RealmTime, Task> asyncCb;
        private readonly int total;
        private int remain;
        private bool destroy;
        private readonly bool isAsync;

        public int Remaining { get { return remain; } }

        public WorldTimer(int tickMs, Action<World, RealmTime> callback)
        {
            remain = total = tickMs;
            cb = callback;
            isAsync = false;
            _logger = Program.Services?.GetRequiredService<ILogger<WorldTimer>>();
        }

        public WorldTimer(int tickMs, Func<World, RealmTime, Task> asyncCallback)
        {
            remain = total = tickMs;
            asyncCb = asyncCallback;
            isAsync = true;
            _logger = Program.Services?.GetRequiredService<ILogger<WorldTimer>>();
        }

        public void Reset()
        {
            remain = total;
        }

        public void Destroy()
        {
            destroy = true;
        }

        public int RemainingInSeconds()
        {
            return (int)TimeSpan.FromMilliseconds(remain).TotalSeconds;
        }

        public bool Tick(World world, RealmTime time)
        {
            if (destroy)
            {
                world.Timers.Remove(this);
                return true;
            }
            remain -= time.thisTickTimes;
            if (remain < 0)
            {
                try
                {
                    if (isAsync)
                    {
                        // Fire and forget for async callbacks
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await asyncCb(world, time).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                _logger?.LogError(ex, "Error in async WorldTimer callback");
                            }
                        });
                    }
                    else
                    {
                        cb(world, time);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error in WorldTimer callback");
                }
                return true;
            }
            return false;
        }
    }
}
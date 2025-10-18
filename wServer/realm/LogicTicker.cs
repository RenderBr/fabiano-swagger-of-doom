#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RageRealm.Shared.Models;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm
{
    public class LogicTicker
    {
        private readonly ILogger<LogicTicker> _logger;
        public static RealmTime CurrentTime;

        private readonly ConcurrentQueue<Action<RealmTime>>[] _pendings;
        private readonly ConcurrentQueue<Func<RealmTime, Task>>[] _asyncPendings;
        private readonly RealmManager _manager;

        public int MsPT { get; }
        public int TPS { get; }

        public LogicTicker(RealmManager manager)
        {
            _logger = Program.Services?.GetRequiredService<ILogger<LogicTicker>>();
            _manager = manager;
            TPS = manager.TPS;
            MsPT = 1000 / TPS;

            _pendings = new ConcurrentQueue<Action<RealmTime>>[Enum.GetValues(typeof(PendingPriority)).Length];
            _asyncPendings = new ConcurrentQueue<Func<RealmTime, Task>>[Enum.GetValues(typeof(PendingPriority)).Length];
            for (int i = 0; i < _pendings.Length; i++)
            {
                _pendings[i] = new ConcurrentQueue<Action<RealmTime>>();
                _asyncPendings[i] = new ConcurrentQueue<Func<RealmTime, Task>>();
            }
        }

        public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority = PendingPriority.Normal)
        {
            _pendings[(int)priority].Enqueue(callback);
        }

        public void AddPendingAsyncAction(Func<RealmTime, Task> callback,
            PendingPriority priority = PendingPriority.Normal)
        {
            _asyncPendings[(int)priority].Enqueue(callback);
        }

        /// <summary>
        /// Main game logic loop. Runs continuously until cancellation is requested.
        /// </summary>
        public void TickLoop(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Logic loop started.");

            var watch = Stopwatch.StartNew();
            long dt = 0;
            long count = 0;
            RealmTime t = new RealmTime();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    long times = dt / MsPT;
                    dt -= times * MsPT;
                    times++;

                    long elapsed = watch.ElapsedMilliseconds;

                    count += times;
                    if (times > 3)
                        _logger?.LogWarning(
                            "LAGGED! times={Times} dt={Dt} count={Count} elapsed={Elapsed} tps={Tps:F2}",
                            times, dt, count, elapsed, count / (elapsed / 1000.0));

                    t.tickTimes = elapsed;
                    t.tickCount = count;
                    t.thisTickCounts = (int)times;
                    t.thisTickTimes = (int)(times * MsPT);

                    // run pending actions synchronously
                    foreach (var queue in _pendings)
                        while (queue.TryDequeue(out var callback))
                            SafeInvoke(() => callback(t));

                    // run async actions *fire-and-forget* (don’t await them)
                    foreach (var queue in _asyncPendings)
                        while (queue.TryDequeue(out var callback))
                            _ = Task.Run(() => SafeInvokeAsync(() => callback(t)));

                    // tick worlds
                    TickWorlds(t);

                    // cleanups (trades, guilds)
                    try
                    {
                        var tradingPlayers = TradeManager.TradingPlayers
                            .Where(p => p.Owner == null)
                            .ToArray();

                        foreach (var player in tradingPlayers)
                            TradeManager.TradingPlayers.Remove(player);

                        var requestPairs = TradeManager.CurrentRequests
                            .Where(p => p.Key.Owner == null || p.Value.Owner == null)
                            .ToArray();

                        foreach (var pair in requestPairs)
                            TradeManager.CurrentRequests.Remove(pair);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error cleaning trade managers");
                    }

                    SafeInvoke(() => GuildManager.Tick(CurrentTime));

                    // fixed sleep
                    long elapsedTick = watch.ElapsedMilliseconds - elapsed;
                    int sleep = (int)(MsPT - elapsedTick);
                    if (sleep > 0)
                        Thread.Sleep(sleep);

                    dt += Math.Max(0, watch.ElapsedMilliseconds - elapsed - MsPT);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Logic loop crashed");
            }
            finally
            {
                _logger?.LogInformation("Logic loop stopped.");
            }
        }

        private void SafeInvoke(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in pending action");
            }
        }

        private async Task SafeInvokeAsync(Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in async pending action");
            }
        }


        private void TickWorlds(RealmTime t)
        {
            CurrentTime = t;
            foreach (var world in _manager.Worlds.Values.Distinct())
            {
                try
                {
                    world.Tick(t);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error ticking world {WorldName} (ID={WorldId})", world.Name, world.Id);
                }
            }
        }
    }
}
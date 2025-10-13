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

        public void AddPendingAsyncAction(Func<RealmTime, Task> callback, PendingPriority priority = PendingPriority.Normal)
        {
            _asyncPendings[(int)priority].Enqueue(callback);
        }

        /// <summary>
        /// Main game logic loop. Runs continuously until cancellation is requested.
        /// </summary>
        public async Task TickLoop(CancellationToken cancellationToken)
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
                        _logger?.LogWarning("LAGGED! times={Times} dt={Dt} count={Count} elapsed={Elapsed} tps={Tps:F2}", times, dt, count, elapsed, count / (elapsed / 1000.0));

                    t.tickTimes = elapsed;
                    t.tickCount = count;
                    t.thisTickCounts = (int)times;
                    t.thisTickTimes = (int)(times * MsPT);

                    // process pending sync actions
                    foreach (var queue in _pendings)
                    {
                        while (queue.TryDequeue(out var callback))
                        {
                            try
                            {
                                callback(t);
                            }
                            catch (Exception ex)
                            {
                                _logger?.LogError(ex, "Error in pending action");
                            }
                        }
                    }

                    // process pending async actions
                    var asyncTasks = new List<Task>();
                    foreach (var queue in _asyncPendings)
                    {
                        while (queue.TryDequeue(out var callback))
                        {
                            try
                            {
                                var task = callback(t);
                                asyncTasks.Add(task);
                            }
                            catch (Exception ex)
                            {
                                _logger?.LogError(ex, "Error starting async pending action");
                            }
                        }
                    }

                    // await all async actions to complete
                    if (asyncTasks.Count > 0)
                    {
                        try
                        {
                            await Task.WhenAll(asyncTasks).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Error in async pending actions");
                        }
                    }

                    // tick worlds
                    try
                    {
                        TickWorlds(t);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error ticking worlds");
                    }

                    // cleanup orphaned trade players
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

                    // tick guild manager
                    try
                    {
                        GuildManager.Tick(CurrentTime);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error ticking guild manager");
                    }

                    // wait until next tick
                    await Task.Delay(MsPT, cancellationToken).ConfigureAwait(false);

                    // measure drift
                    dt += Math.Max(0, watch.ElapsedMilliseconds - elapsed - MsPT);
                }
            }
            catch (OperationCanceledException)
            {
                // expected on shutdown
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

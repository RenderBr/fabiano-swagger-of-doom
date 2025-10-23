#region

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RageRealm.Shared.Models;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm
{
    public class LogicTicker
    {
        private const int MaxCatchUpTicks = 5;
        private readonly ILogger<LogicTicker> _logger;
        public static RealmTime CurrentTime;

        private readonly ConcurrentQueue<Action<RealmTime>>[] _pendings;
        private readonly ConcurrentQueue<Func<RealmTime, Task>>[] _asyncPendings;
        private readonly RealmManager _manager;

        public int MsPT { get; }
        public int TPS { get; }
        private readonly long _tickLength;

        public LogicTicker(RealmManager manager)
        {
            _logger = Program.Services?.GetRequiredService<ILogger<LogicTicker>>();
            _manager = manager;
            TPS = manager.TPS;
            var msPerTick = 1000.0 / TPS;
            MsPT = Math.Max(1, (int)Math.Round(msPerTick));
            _tickLength = (long)Math.Round(Stopwatch.Frequency / (double)TPS);

            _pendings = new ConcurrentQueue<Action<RealmTime>>[Enum.GetValues(typeof(PendingPriority)).Length];
            _asyncPendings = new ConcurrentQueue<Func<RealmTime, Task>>[Enum.GetValues(typeof(PendingPriority)).Length];
            for (int i = 0; i < _pendings.Length; i++)
            {
                _pendings[i] = new ConcurrentQueue<Action<RealmTime>>();
                _asyncPendings[i] = new ConcurrentQueue<Func<RealmTime, Task>>();
            }
        }

        public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority = PendingPriority.Normal)
            => _pendings[(int)priority].Enqueue(callback);

        public void AddPendingAsyncAction(Func<RealmTime, Task> callback, PendingPriority priority = PendingPriority.Normal)
            => _asyncPendings[(int)priority].Enqueue(callback);

        public void TickLoop(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Logic loop started.");

            var stopwatch = Stopwatch.StartNew();
            long tickCount = 0;
            long simulatedTimeMs = 0;
            var time = new RealmTime();

            var previousTimestamp = stopwatch.ElapsedTicks;
            double accumulator = 0;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var currentTimestamp = stopwatch.ElapsedTicks;
                    var deltaTicks = currentTimestamp - previousTimestamp;
                    if (deltaTicks < 0)
                        deltaTicks = 0;

                    previousTimestamp = currentTimestamp;
                    accumulator += deltaTicks;

                    var processedTicks = 0;
                    while (accumulator >= _tickLength && !cancellationToken.IsCancellationRequested)
                    {
                        ProcessTick(ref time, ref tickCount, ref simulatedTimeMs);
                        accumulator -= _tickLength;
                        processedTicks++;

                        if (processedTicks >= MaxCatchUpTicks)
                            break;
                    }

                    if (processedTicks >= MaxCatchUpTicks && accumulator >= _tickLength)
                    {
                        accumulator %= _tickLength;
                        simulatedTimeMs = tickCount * MsPT;
                    }

                    if (processedTicks == 0)
                    {
                        var ticksUntilNext = _tickLength - accumulator;
                        var msUntilNext = ticksUntilNext * 1000.0 / Stopwatch.Frequency;

                        if (msUntilNext > 1)
                        {
                            Thread.Sleep(Math.Max(1, (int)Math.Floor(msUntilNext - 0.5)));
                        }
                        else
                        {
                            Thread.SpinWait(50);
                        }
                    }
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

        private void ProcessTick(ref RealmTime time, ref long tickCount, ref long simulatedTimeMs)
        {
            tickCount++;
            simulatedTimeMs += MsPT;

            time.tickCount = tickCount;
            time.tickTimes = simulatedTimeMs;
            time.thisTickCounts = 1;
            time.thisTickTimes = MsPT;

            var tickTime = time;

            RunPendingActions(tickTime);
            RunPendingAsyncActions(tickTime);
            TickWorlds(tickTime);
            CleanupTrades();
            SafeInvoke(() => GuildManager.Tick(tickTime));
        }

        private void RunPendingActions(RealmTime t)
        {
            foreach (var queue in _pendings)
                while (queue.TryDequeue(out var callback))
                    SafeInvoke(() => callback(t));
        }

        private void RunPendingAsyncActions(RealmTime t)
        {
            foreach (var queue in _asyncPendings)
                while (queue.TryDequeue(out var callback))
                    _ = Task.Run(() => SafeInvokeAsync(() => callback(t)));
        }

        private void CleanupTrades()
        {
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
        }

        private void SafeInvoke(Action action)
        {
            try { action(); }
            catch (Exception ex) { _logger?.LogError(ex, "Error in pending action"); }
        }

        private async Task SafeInvokeAsync(Func<Task> func)
        {
            try { await func(); }
            catch (Exception ex) { _logger?.LogError(ex, "Error in async pending action"); }
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

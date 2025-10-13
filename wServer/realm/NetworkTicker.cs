#region

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using wServer.networking;

#endregion

namespace wServer.realm
{
    using Work = Tuple<Client, Packet>;

    public class NetworkTicker
    {
        private static readonly ConcurrentQueue<Work> _pending = new();
        private static readonly SpinWait _loopLock = new();
        private readonly ILogger<NetworkTicker> _logger;

        public RealmManager Manager { get; }

        public NetworkTicker(RealmManager manager)
        {
            _logger = Program.Services?.GetRequiredService<ILogger<NetworkTicker>>();
            Manager = manager;
        }

        public void AddPendingPacket(Client parent, Packet pkt)
        {
            _pending.Enqueue(new Work(parent, pkt));
        }

        public async Task TickLoop(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Network loop started.");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _loopLock.Reset();

                    while (_pending.TryDequeue(out var work))
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        var client = work.Item1;
                        var packet = work.Item2;

                        try
                        {
                            if (client.Stage == ProtocalStage.Disconnected)
                            {
                                if (client.Account?.AccountId is string accId)
                                    Manager.Clients.TryRemove(accId, out _);
                                continue;
                            }

                            await client.ProcessPacket(packet).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Error processing packet for {ClientName}", client?.Account?.Name ?? "unknown");
                        }
                    }

                    // Sleep only when no pending packets
                    if (_pending.IsEmpty && !cancellationToken.IsCancellationRequested)
                        await Task.Delay(1, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // graceful stop
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in Network loop");
            }

            _logger?.LogInformation("Network loop stopped.");
        }
    }
}

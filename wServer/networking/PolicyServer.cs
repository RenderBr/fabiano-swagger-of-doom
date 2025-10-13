#region

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace wServer.networking
{
    internal class PolicyServer : IAsyncDisposable
    {
        private readonly ILogger<PolicyServer> _logger;
        private readonly TcpListener _listener;
        private CancellationTokenSource? _cts;
        private Task? _listenerTask;
        private bool _started;

        public PolicyServer()
        {
            _logger = Program.Services?.GetRequiredService<ILogger<PolicyServer>>();
            _listener = new TcpListener(IPAddress.Any, 843);
        }

        public async Task StartAsync()
        {
            if (_started) return;

            try
            {
                _logger?.LogInformation("Starting policy server...");
                _listener.Start();
                _cts = new CancellationTokenSource();
                _listenerTask = RunAsync(_cts.Token);
                _started = true;
                await Task.CompletedTask;
            }
            catch (SocketException ex)
            {
                _logger?.LogWarning(ex, "Could not start Socket Policy Server, is port 843 occupied?");
                _started = false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error starting policy server");
                _started = false;
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync(cancellationToken);
                    _ = Task.Run(() => HandleClientAsync(client, cancellationToken), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break; // graceful stop
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error accepting client");
                    await Task.Delay(100, cancellationToken);
                }
            }
        }

        private static async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            try
            {
                await using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, leaveOpen: true);
                await using var writer = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true)
                {
                    AutoFlush = true
                };

                // Read until null terminator or timeout
                var buffer = new char[1024];
                var sb = new StringBuilder();
                int read;

                stream.ReadTimeout = 5000;

                while ((read = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    sb.Append(buffer, 0, read);
                    if (sb.ToString().Contains('\0'))
                        break;
                }

                var request = sb.ToString().TrimEnd('\0', '\r', '\n');

                if (request == "<policy-file-request/>")
                {
                    const string policy =
                        @"<cross-domain-policy>
    <allow-access-from domain=""*"" to-ports=""*"" />
</cross-domain-policy>";

                    await writer.WriteAsync(policy + "\r\n");
                }
            }
            catch (IOException)
            {
                // client disconnected
            }
            catch (Exception ex)
            {
                Program.Services.GetRequiredService<ILogger<PolicyServer>>()
                    .LogError(ex, "Error handling policy client");
            }
            finally
            {
                client.Close();
            }
        }

        public async Task StopAsync()
        {
            if (!_started) return;

            try
            {
                _logger?.LogInformation("Stopping policy server...");
                _cts?.Cancel();
                _listener.Stop();

                if (_listenerTask != null)
                    await _listenerTask;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error stopping policy server");
            }
            finally
            {
                _started = false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            _cts?.Dispose();
        }
    }
}
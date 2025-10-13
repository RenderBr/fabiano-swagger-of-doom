#region

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using wServer.realm;

#endregion

namespace wServer.networking
{
    internal class Server : IDisposable
    {
        public const string VERSION = "1.0.3.0";
        private readonly ILogger<Server> _logger;
        private bool _running;

        public Server(RealmManager manager)
        {
            _logger = Program.Services?.GetRequiredService<ILogger<Server>>();
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Manager = manager;
        }

        public Socket Socket { get; }
        public RealmManager Manager { get; }

        public async Task StartAsync()
        {
            _logger?.LogInformation("Starting server...");
            Socket.Bind(new IPEndPoint(IPAddress.Any, Program.Config.Realm.ServerPort));
            Socket.Listen(255);
            _running = true;

            // accept loop
            _ = Task.Run(AcceptLoopAsync);
            await Task.CompletedTask;
        }

        private async Task AcceptLoopAsync()
        {
            while (_running)
            {
                try
                {
                    var skt = await Socket.AcceptAsync().ConfigureAwait(false);
                    _ = Task.Run(() => new Client(Manager, skt));
                }
                catch (ObjectDisposedException)
                {
                    // occurs during shutdown
                    break;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error accepting connection");
                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
        }

        public async Task StopAsync()
        {
            _logger?.LogInformation("Stopping server...");
            _running = false;

            try
            {
                Socket.Close();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error closing socket");
            }

            // save & disconnect all clients
            var clients = Manager.Clients.Values.ToArray();
            foreach (var client in clients)
            {
                try
                {
                    await client.Save().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Failed to save client {ClientName}", client?.Account?.Name);
                }

                client?.Disconnect();
            }
        }

        public void Dispose()
        {
            _running = false;
            Socket?.Dispose();
        }

    }
}
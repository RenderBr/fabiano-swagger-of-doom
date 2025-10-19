#region

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer.Factories;
using wServer.realm;

#endregion

namespace wServer.networking
{
    public class Server(ILogger<Server> logger, RealmManager manager, IClientFactory clientFactory) : IDisposable
    {
        public const string VERSION = "1.0.3.0";
        private bool _running;

        public Socket Socket { get; } = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public RealmManager Manager { get; } = manager;

        public async Task StartAsync()
        {
            logger?.LogInformation("Starting server...");
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
                    _ = Task.Run(() => clientFactory.CreateClient(Program.Services, skt));
                }
                catch (ObjectDisposedException)
                {
                    // occurs during shutdown
                    break;
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Error accepting connection");
                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
        }

        public async Task StopAsync()
        {
            logger?.LogInformation("Stopping server...");
            _running = false;

            try
            {
                Socket.Close();
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error closing socket");
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
                    logger?.LogWarning(ex, "Failed to save client {ClientName}", client?.Account?.Name);
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
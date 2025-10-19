using System;
using wServer.networking;
using wServer.realm;

namespace wServer.Factories;

public interface IClientFactory
{
    public Client CreateClient(IServiceProvider serviceProvider, System.Net.Sockets.Socket socket);
}
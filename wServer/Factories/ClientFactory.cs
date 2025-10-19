using System;
using System.Net.Sockets;
using wServer.networking;

namespace wServer.Factories;

public class ClientFactory(IServiceProvider serviceProvider) : IClientFactory
{
    public Client CreateClient(IServiceProvider serviceProvider, Socket socket)
    {
        return new Client(serviceProvider, socket);
    }
}
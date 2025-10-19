using System;
using System.Net.Sockets;
using wServer.networking;

namespace wServer.Factories;

public class NetworkHandlerFactory(IServiceProvider serviceProvider) : INetworkHandlerFactory
{
    public NetworkHandler CreateNetworkHandler(IServiceProvider services, Client client, Socket socket)
    {
        return new NetworkHandler(services, client, socket);
    }
}
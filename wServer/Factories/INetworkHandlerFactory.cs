using System;
using System.Net.Sockets;
using wServer.networking;

namespace wServer.Factories;

public interface INetworkHandlerFactory
{
    public NetworkHandler CreateNetworkHandler(IServiceProvider services, Client client, Socket socket);
}
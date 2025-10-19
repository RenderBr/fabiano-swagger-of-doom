using System;
using wServer.networking;
using wServer.networking.cliPackets;

namespace wServer.Factories;

public class PacketHandlerFactory(IServiceProvider serviceProvider) : IPacketHandlerFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public PacketHandlerBase<T> GetHandler<T>() where T : ClientPacket
    {
        throw new System.NotImplementedException();
    }
}
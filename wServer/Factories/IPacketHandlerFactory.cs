using System;
using wServer.networking;
using wServer.networking.cliPackets;

namespace wServer.Factories;

public interface IPacketHandlerFactory
{
    public PacketHandlerBase<T> GetHandler<T>() where T : ClientPacket;
}
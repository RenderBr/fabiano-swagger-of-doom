#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using FailurePacket = wServer.networking.svrPackets.FailurePacket;
using wServer.realm;

#endregion

namespace wServer.networking
{
    public interface IPacketHandler
    {
        PacketID ID { get; }
        Task Handle(Client client, ClientPacket packet);
    }

    public abstract class PacketHandlerBase<T>(IServiceProvider serviceProvider) : IPacketHandler
        where T : ClientPacket
    {
        protected IServiceProvider ServiceProvider => serviceProvider;

        private Client client;

        public abstract PacketID ID { get; }

        public Task Handle(Client client, ClientPacket packet)
        {
            this.client = client;
            return HandlePacket(client, (T)packet);
        }

        public RealmManager Manager
        {
            get { return client.Manager; }
        }

        public Client Client
        {
            get { return client; }
        }

        protected abstract Task HandlePacket(Client client, T packet);

        protected void SendFailure(string text)
        {
            client.SendPacket(new FailurePacket { ErrorId = 0, ErrorDescription = text });
        }
    }

    public class PacketHandlers
    {
        public Dictionary<PacketID, IPacketHandler> Handlers = new Dictionary<PacketID, IPacketHandler>();

        public PacketHandlers(IServiceProvider serviceProvider)
        {
            foreach (Type i in typeof(Packet).Assembly.GetTypes())
            {
                if (typeof(IPacketHandler).IsAssignableFrom(i) &&
                    !i.IsAbstract && !i.IsInterface)
                {
                    IPacketHandler pkt = (IPacketHandler)Activator.CreateInstance(i, serviceProvider);
                    Handlers.Add(pkt.ID, pkt);
                }
            }
        }
    }
}
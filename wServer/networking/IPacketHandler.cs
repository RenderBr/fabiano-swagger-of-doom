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
    internal interface IPacketHandler
    {
        PacketID ID { get; }
        Task Handle(Client client, ClientPacket packet);
    }

    internal abstract class PacketHandlerBase<T> : IPacketHandler where T : ClientPacket
    {
        protected ILogger _logger;
        private Client client;

        public PacketHandlerBase()
        {
            _logger = Program.Services?.GetService<ILoggerFactory>()?.CreateLogger(GetType());
        }

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

    internal class PacketHandlers
    {
        public static Dictionary<PacketID, IPacketHandler> Handlers = new Dictionary<PacketID, IPacketHandler>();

        static PacketHandlers()
        {
            foreach (Type i in typeof(Packet).Assembly.GetTypes())
            {
                if (typeof(IPacketHandler).IsAssignableFrom(i) &&
                    !i.IsAbstract && !i.IsInterface)
                {
                    IPacketHandler pkt = (IPacketHandler)Activator.CreateInstance(i);
                    Handlers.Add(pkt.ID, pkt);
                }
            }
        }
    }
}
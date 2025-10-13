#region

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer.networking.cliPackets;

#endregion

namespace wServer.networking.handlers
{
    internal class RequestTradeHandler : PacketHandlerBase<RequestTradePacket>
    {
        public override PacketID ID
        {
            get { return PacketID.REQUESTTRADE; }
        }

        protected override Task HandlePacket(Client client, RequestTradePacket packet)
        {
            client.Manager.Logic.AddPendingAction(t => client.Player.RequestTrade(t, packet));
            return Task.CompletedTask;
        }
    }

    internal class ChangeTradeHandler : PacketHandlerBase<ChangeTradePacket>
    {
        public override PacketID ID
        {
            get { return PacketID.CHANGETRADE; }
        }

        protected override Task HandlePacket(Client client, ChangeTradePacket packet)
        {
            if (client?.Player == null)
            {
                Program.Logger.LogWarning("Received ChangeTradePacket from null player");
                return Task.CompletedTask; // Player not loaded or disconnected
            }

            client.Manager?.Logic.AddPendingAction(t => { client.Player?.ChangeTrade(t, packet); });
            return Task.CompletedTask;
        }
    }

    internal class AcceptTradeHandler : PacketHandlerBase<AcceptTradePacket>
    {
        public override PacketID ID
        {
            get { return PacketID.ACCEPTTRADE; }
        }

        protected override Task HandlePacket(Client client, AcceptTradePacket packet)
        {
            client.Manager.Logic.AddPendingAction(t => client.Player.AcceptTrade(t, packet));
            return Task.CompletedTask;
        }
    }

    internal class CancelTradeHandler : PacketHandlerBase<CancelTradePacket>
    {
        public override PacketID ID
        {
            get { return PacketID.CANCELTRADE; }
        }

        protected override Task HandlePacket(Client client, CancelTradePacket packet)
        {
            client.Manager.Logic.AddPendingAction(t => client.Player.CancelTrade(t, packet));
            return Task.CompletedTask;
        }
    }
}
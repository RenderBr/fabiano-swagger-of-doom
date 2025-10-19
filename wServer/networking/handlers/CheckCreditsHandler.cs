#region

using System;
using System.Threading.Tasks;
using db;
using RageRealm.Shared.Models;
using wServer.networking.cliPackets;
using wServer.realm;

#endregion

namespace wServer.networking.handlers
{
    internal class CheckCreditsHandler(IServiceProvider serviceProvider) : PacketHandlerBase<CheckCreditsPacket>(serviceProvider)
    {
        public override PacketID ID
        {
            get { return PacketID.CHECKCREDITS; }
        }

        protected override async Task HandlePacket(Client client, CheckCreditsPacket packet)
        {
            client.Manager.Logic.AddPendingAction(t =>
            {
                client.Manager.Database.DoActionAsync( db =>
                {
                    // Read stats equivalent would be done via repository
                    client.Player.Credits = client.Account.Credits;
                    client.Player.UpdateCount++;
                });
            }, PendingPriority.Networking);
            
            await Task.CompletedTask;
        }
    }
}
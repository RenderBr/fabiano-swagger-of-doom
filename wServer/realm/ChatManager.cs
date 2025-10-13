using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using wServer.realm.entities;
using wServer.networking.svrPackets;
using wServer.realm.entities.player;
using wServer.networking;

namespace wServer.realm
{
    public class ChatManager
    {
        private readonly ILogger<ChatManager> _logger;
        RealmManager manager;
        public ChatManager(RealmManager manager)
        {
            _logger = Program.Services?.GetRequiredService<ILogger<ChatManager>>();
            this.manager = manager;
        }

        public void Say(Player src, string text)
        {
            src.BroadcastSync(new TextPacket()
            {
                Name = (src.Client.Account.Rank >= 2 ? "@" : src.Client.Account.Rank >= 1 ? "#" : "") + src.Name,
                ObjectId = src.Id,
                Stars = src.Stars,
                BubbleTime = 10,
                Recipient = "",
                Text = text.ToSafeText(),
                CleanText = text.ToSafeText()
            }, p => !p.Ignored.Contains(src.AccountId));
            _logger?.LogInformation("[{WorldName}({WorldId})] <{PlayerName}> {Message}", src.Owner.Name, src.Owner.Id, src.Name, text);
            src.Owner.ChatReceived(src, text);
        }

        public void SayGuild(Player src, string text)
        {
            foreach (Client i in src.Manager.Clients.Values)
            {
                if (String.Equals(src.Guild, i.Player.Guild))
                {
                    i.SendPacket(new TextPacket()
                    {
                        Name = src.ResolveGuildChatName(),
                        ObjectId = src.Id,
                        Stars = src.Stars,
                        BubbleTime = 10,
                        Recipient = "*Guild*",
                        Text = text.ToSafeText(),
                        CleanText = text.ToSafeText()
                    });
                }
            }
        }

        public void News(string text)
        {
            foreach (var i in manager.Clients.Values)
                i.SendPacket(new TextPacket()
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@NEWS",
                    Text = text.ToSafeText()
                });
            _logger?.LogInformation("<NEWS> {NewsText}", text);
        }

        public void Announce(string text)
        {
            foreach (var i in manager.Clients.Values)
                i.SendPacket(new TextPacket()
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@Announcement",
                    Text = text.ToSafeText()
                });
            _logger?.LogInformation("<Announcement> {AnnouncementText}", text);
        }

        public void Oryx(World world, string text)
        {
            world.BroadcastPacket(new TextPacket()
            {
                BubbleTime = 0,
                Stars = -1,
                Name = "#Oryx the Mad God",
                Text = text.ToSafeText()
            }, null);
            _logger?.LogInformation("[{WorldName}({WorldId})] <Oryx the Mad God> {OryxMessage}", world.Name, world.Id, text);
        }
    }
}

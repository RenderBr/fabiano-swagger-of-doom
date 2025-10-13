#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using db;
using RageRealm.Shared.Models;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm.entities;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm.commands
{
    internal class ShowGiftCode : Command
    {
        public ShowGiftCode()
            : base("giftcode")
        {
        }

        protected override async Task Process(Player player, RealmTime time, string[] args)
        {
            var giftCode = player.Client.Account.NextGiftCode; // property in Account model
            if (giftCode == null)
            {
                player.SendError("No new giftcode found.");
                return;
            }

            var data = AccountDataHelper.GenerateAccountGiftCodeData(player.AccountId, giftCode).Write();
            var qrGenerator = new QrCodeGenerator();
            var qrCode = qrGenerator.CreateQrCode($"{Program.Config.Realm.ServerDomain}/account/redeemGiftCode?data={data}", QrCodeGenerator.EccLevel.H);
            var renderer = new QrCodeGenerator.QrCode.QrCodeRenderer
            {
                ModuleMatrix = qrCode.ModuleMatrix
                    .Select(row => row.Cast<bool>().ToList())
                    .ToList()
            };

            using var bmp = renderer.GetGraphic(8); // 8px per module = decent density
            var rgbValues = bmp.GetPixels();

            player.Client.SendPacket(new PicPacket
            {
                BitmapData = new BitmapData
                {
                    Bytes = rgbValues,
                    Height = bmp.Height,
                    Width = bmp.Width
                }
            });
        }
    }

    internal class TutorialCommand : Command
    {
        public TutorialCommand()
            : base("tutorial")
        {
        }

        protected override async Task Process(Player player, RealmTime time, string[] args)
        {
            player.Client.Reconnect(new ReconnectPacket
            {
                Host = "",
                Port = Program.Config.Realm.ServerPort,
                GameId = World.TUT_ID,
                Name = "Tutorial",
                Key = Empty<byte>.Array,
            });
        }
    }

    internal class TradeCommand : Command
    {
        public TradeCommand()
            : base("trade")
        {
        }

        protected override async Task Process(Player player, RealmTime time, string[] args)
        {
            if(String.IsNullOrWhiteSpace(args[0]))
            {
                player.SendInfo("Usage: /trade <player name>");
                return;
            }
            player.RequestTrade(time, new RequestTradePacket
            {
                Name = args[0]
            });
        }
    }


    internal class WhoCommand : Command
    {
        public WhoCommand()
            : base("who")
        {
        }

        protected override async Task Process(Player player, RealmTime time, string[] args)
        {
            StringBuilder sb = new StringBuilder("Players online: ");
            Player[] copy = player.Owner.Players.Values.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                if (i != 0) sb.Append(", ");
                sb.Append(copy[i].Name);
            }

            player.SendInfo(sb.ToString());
        }
    }

    internal class ServerCommand : Command
    {
        public ServerCommand()
            : base("server")
        {
        }

        protected override async Task Process(Player player, RealmTime time, string[] args)
        {
            player.SendInfo(player.Owner.Name);
        }
    }

    internal class PauseCommand : Command
    {
        public PauseCommand()
            : base("pause")
        {
        }

        protected override async Task Process(Player player, RealmTime time, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffectIndex.Paused))
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paused,
                    DurationMS = 0
                });
                player.SendInfo("Game resumed.");
            }
            else
            {
                foreach (Enemy i in player.Owner.EnemiesCollision.HitTest(player.X, player.Y, 8).OfType<Enemy>())
                {
                    if (i.ObjectDesc.Enemy)
                    {
                        player.SendInfo("Not safe to pause.");
                        return;
                    }
                }
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paused,
                    DurationMS = -1
                });
                player.SendInfo("Game paused.");
            }
        }
    }

    internal class TeleportCommand : Command
    {
        public TeleportCommand()
            : base("teleport")
        {
        }

        protected override async Task Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (String.Equals(player.Name.ToLower(), args[0].ToLower()))
                {
                    player.SendInfo("You are already at yourself, and always will be!");
                    return;
                }

                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        player.Teleport(time, new TeleportPacket
                        {
                            ObjectId = i.Value.Id
                        });
                        return;
                    }
                }
                player.SendInfo(string.Format("Cannot teleport, {0} not found!", args[0].Trim()));
            }
            catch
            {
                player.SendHelp("Usage: /teleport <player name>");
            }
        }
    }

    class TellCommand : Command
    {
        public TellCommand() : base("tell") { }

        protected override async Task Process(Player player, RealmTime time, string[] args)
        {
            if (!player.NameChosen)
            {
                player.SendError("Choose a name!");
                return;
            }
            if (args.Length < 2)
            {
                player.SendError("Usage: /tell <player name> <text>");
                return;
            }

            string playername = args[0].Trim();
            string msg = string.Join(" ", args, 1, args.Length - 1);

            if (String.Equals(player.Name.ToLower(), playername.ToLower()))
            {
                player.SendInfo("Quit telling yourself!");
                return;
            }

            if (playername.ToLower() == "muledump")
            {
                if (msg.ToLower() == "private muledump")
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Recipient = "Muledump",
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });

                    // TODO: Migrate muledump functionality to repository pattern
                    // player.Manager.Database.DoActionAsync(db =>
                    // {
                    //     var cmd = db.CreateQuery();
                    //     cmd.CommandText = "UPDATE accounts SET publicMuledump=0 WHERE id=@accId;";
                    //     cmd.Parameters.AddWithValue("@accId", player.AccountId);
                    //     cmd.ExecuteNonQuery();
                    // });

                    player.Client.SendPacket(new TextPacket()
                    {
                        ObjectId = -1,
                        BubbleTime = 10,
                        Stars = 70,
                        Name = "Muledump",
                        Recipient = player.Name,
                        Text = "Your muledump is now hidden, only you can view it now.",
                        CleanText = ""
                    });
                }
                else if (msg.ToLower() == "public muledump")
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Recipient = "Muledump",
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });
                    
                    // TODO: Migrate muledump functionality to repository pattern
                    // player.Manager.Database.DoActionAsync(db =>
                    // {
                    //     var cmd = db.CreateQuery();
                    //     cmd.CommandText = "UPDATE accounts SET publicMuledump=1 WHERE id=@accId;";
                    //     cmd.Parameters.AddWithValue("@accId", player.AccountId);
                    //     cmd.ExecuteNonQuery();
                    // });
                    
                    player.Client.SendPacket(new TextPacket()
                    {
                        ObjectId = -1,
                        BubbleTime = 10,
                        Stars = 70,
                        Name = "Muledump",
                        Recipient = player.Name,
                        Text = "Your muledump is now public, anyone can view it now.",
                        CleanText = ""
                    });
                }
                else
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Recipient = "Muledump",
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });

                    player.Client.SendPacket(new TextPacket()
                    {
                        ObjectId = -1,
                        BubbleTime = 10,
                        Stars = 70,
                        Name = "Muledump",
                        Recipient = player.Name,
                        Text = "U WOT M8, 1v1 IN THE GARAGE!!!!111111oneoneoneeleven",
                        CleanText = ""
                    });
                }
                return;
            }

            foreach (var i in player.Manager.Clients.Values)
            {
                if (i.Account.NameChosen && i.Account.Name.EqualsIgnoreCase(playername))
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Recipient = i.Account.Name,
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });

                    i.SendPacket(new TextPacket() //echo to /tell player
                    {
                        ObjectId = i.Player.Owner.Id == player.Owner.Id ? player.Id : -1,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Recipient = i.Account.Name,
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });
                    return;
                }
            }
            player.SendError(string.Format("{0} not found.", playername));
        }
    }
}
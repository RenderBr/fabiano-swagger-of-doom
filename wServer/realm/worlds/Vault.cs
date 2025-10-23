#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using RageRealm.Shared.Models;
using wServer;
using wServer.networking;
using wServer.realm.entities;

#endregion

namespace wServer.realm.worlds
{
    public sealed class Vault : World
    {
        private readonly ConcurrentDictionary<Tuple<Container, VaultChest>, int> _vaultChests =
            new ConcurrentDictionary<Tuple<Container, VaultChest>, int>();

        private readonly bool isLimbo;
        private Client psr;
        public string AccountId { get; private set; }

        public Vault(bool isLimbo, Client psr = null, RealmManager manager = null,
            ILogger<World> logger = null, RealmPortalMonitor portalMonitor = null, GeneratorCache generatorCache = null)
            : base(manager ?? psr?.Manager, logger, portalMonitor, generatorCache)
        {
            Id = VAULT_ID;
            Name = "Vault";
            ClientWorldName = "server.Vault";
            Background = 2;
            this.psr = psr;
            this.isLimbo = isLimbo;
            ShowDisplays = true;
            if (psr != null)
                AccountId = psr.Account != null ? psr.Account.AccountId : "-1";
            else
                AccountId = "-1";

            Init();
        }

        public string PlayerOwnerName { get; private set; }
        
        protected override void Init()
        {
            if (!(IsLimbo = isLimbo))
            {
                LoadMap("wServer.realm.worlds.maps.vault.wmap", MapType.Wmap);
                if (psr != null)
                    Init(psr);
                else
                    Init(AccountId);
            }
        }

        private void Init(Client psr)
        {
            AccountId = psr.Account.AccountId;
            PlayerOwnerName = psr.Account.Name;

            List<IntPoint> vaultChestPosition = new List<IntPoint>();
            List<IntPoint> giftChestPosition = new List<IntPoint>();
            IntPoint spawn = new IntPoint(0, 0);

            int w = Map.Width;
            int h = Map.Height;
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                WmapTile tile = Map[x, y];
                if (tile.Region == TileRegion.Spawn)
                    spawn = new IntPoint(x, y);
                else if (tile.Region == TileRegion.Vault)
                    vaultChestPosition.Add(new IntPoint(x, y));
                else if (tile.Region == TileRegion.Gifting_Chest)
                    giftChestPosition.Add(new IntPoint(x, y));
            }

            vaultChestPosition.Sort((x, y) => Comparer<int>.Default.Compare(
                (x.X - spawn.X) * (x.X - spawn.X) + (x.Y - spawn.Y) * (x.Y - spawn.Y),
                (y.X - spawn.X) * (y.X - spawn.X) + (y.Y - spawn.Y) * (y.Y - spawn.Y)));

            var chests = psr.Account.Vaults.Select(chest => new VaultChest
            {
                _Items = chest.Items,
                ChestId = chest.ChestId
            }).ToList();

            if (psr.Account.Gifts != null)
            {
                List<GiftChest> giftChests = new List<GiftChest>();
                GiftChest c = new GiftChest();
                c.Items = new List<Item>(8);
                bool wasLastElse = false;
                int[] gifts = string.IsNullOrEmpty(psr.Account.Gifts)
                    ? new int[0]
                    : psr.Account.Gifts.Split(',').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToArray();
                gifts.Shuffle();
                for (int i = 0; i < gifts.Count(); i++)
                {
                    if (Manager.GameDataService.Items.ContainsKey((ushort)gifts[i]))
                    {
                        if (c.Items.Count < 8)
                        {
                            c.Items.Add(Manager.GameDataService.Items[(ushort)gifts[i]]);
                            wasLastElse = false;
                        }
                        else
                        {
                            giftChests.Add(c);
                            c = new GiftChest();
                            c.Items = new List<Item>(8);
                            c.Items.Add(Manager.GameDataService.Items[(ushort)gifts[i]]);
                            wasLastElse = true;
                        }
                    }
                }

                if (!wasLastElse)
                    giftChests.Add(c);

                foreach (GiftChest chest in giftChests)
                {
                    if (giftChestPosition.Count == 0) break;
                    while (chest.Items.Count < 8)
                        chest.Items.Add(null);
                    OneWayContainer con = new OneWayContainer(Manager, 0x0744, null, false);
                    List<Item> inv = chest.Items;
                    for (int j = 0; j < 8; j++)
                        con.Inventory[j] = inv[j];
                    con.Move(giftChestPosition[0].X + 0.5f, giftChestPosition[0].Y + 0.5f);
                    EnterWorld(con);
                    giftChestPosition.RemoveAt(0);
                }
            }

            foreach (VaultChest t in chests)
            {
                if (vaultChestPosition.Count == 0) break;
                Container con = new Container(Manager, 0x0504, null, false);
                Item[] inv =
                    t.Items.Select(_ =>
                            _ == -1
                                ? null
                                : (Manager.GameDataService.Items.ContainsKey((ushort)_)
                                    ? Manager.GameDataService.Items[(ushort)_]
                                    : null))
                        .ToArray();
                for (int j = 0; j < 8; j++)
                    con.Inventory[j] = inv[j];
                con.Move(vaultChestPosition[0].X + 0.5f, vaultChestPosition[0].Y + 0.5f);
                EnterWorld(con);
                vaultChestPosition.RemoveAt(0);

                _vaultChests[new Tuple<Container, VaultChest>(con, t)] = con.UpdateCount;
            }

            foreach (IntPoint i in giftChestPosition)
            {
                StaticObject x = new StaticObject(Manager, 0x0743, null, true, false, false);
                x.Move(i.X + 0.5f, i.Y + 0.5f);
                EnterWorld(x);
            }

            foreach (IntPoint i in vaultChestPosition)
            {
                SellableObject x = new SellableObject(Manager, 0x0505);
                x.Move(i.X + 0.5f, i.Y + 0.5f);
                EnterWorld(x);
            }
        }

        private void Init(string accId)
        {
            Manager.Database.DoActionAsync(async db =>
            {
                this.AccountId = accId;

                var account = await db.GetAccount(int.Parse(accId));
                if (account != null)
                    PlayerOwnerName = account.Name;

                List<IntPoint> vaultChestPosition = new List<IntPoint>();
                IntPoint spawn = new IntPoint(0, 0);

                int w = Map.Width;
                int h = Map.Height;
                for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    WmapTile tile = Map[x, y];
                    if (tile.Region == TileRegion.Spawn)
                        spawn = new IntPoint(x, y);
                    else if (tile.Region == TileRegion.Vault)
                        vaultChestPosition.Add(new IntPoint(x, y));
                }

                vaultChestPosition.Sort((x, y) => Comparer<int>.Default.Compare(
                    (x.X - spawn.X) * (x.X - spawn.X) + (x.Y - spawn.Y) * (x.Y - spawn.Y),
                    (y.X - spawn.X) * (y.X - spawn.X) + (y.Y - spawn.Y) * (y.Y - spawn.Y)));

                List<VaultChest> chests = new List<VaultChest>();

                // Get vaults for this account via repository (placeholder implementation)
                // Note: This needs proper VaultChest mapping from Vault entities
                var vaultEntities = new List<db.Models.Vault>(); // Placeholder
                foreach (var vault in vaultEntities)
                {
                    chests.Add(new VaultChest
                    {
                        _Items = vault.Items,
                        ChestId = vault.ChestId
                    });
                }

                foreach (VaultChest t in chests)
                {
                    Container con = new Container(Manager, 0x0504, null, false);
                    Item[] inv =
                        t.Items.Select(_ =>
                                _ == -1
                                    ? null
                                    : (Manager.GameDataService.Items.ContainsKey((ushort)_)
                                        ? Manager.GameDataService.Items[(ushort)_]
                                        : null))
                            .ToArray();
                    for (int j = 0; j < 8; j++)
                        con.Inventory[j] = inv[j];
                    con.Move(vaultChestPosition[0].X + 0.5f, vaultChestPosition[0].Y + 0.5f);
                    EnterWorld(con);
                    vaultChestPosition.RemoveAt(0);

                    _vaultChests[new Tuple<Container, VaultChest>(con, t)] = con.UpdateCount;
                }

                foreach (IntPoint i in vaultChestPosition)
                {
                    SellableObject x = new SellableObject(Manager, 0x0505);
                    x.Move(i.X + 0.5f, i.Y + 0.5f);
                    EnterWorld(x);
                }
            });
        }

        public void AddChest(VaultChest chest, Entity original)
        {
            Container con = new Container(Manager, 0x0504, null, false);
            Item[] inv =
                chest.Items.Select(_ =>
                        _ == -1
                            ? null
                            : (Manager.GameDataService.Items.ContainsKey((ushort)_)
                                ? Manager.GameDataService.Items[(ushort)_]
                                : null))
                    .ToArray();
            for (int j = 0; j < 8; j++)
                con.Inventory[j] = inv[j];
            con.Move(original.X, original.Y);
            LeaveWorld(original);
            EnterWorld(con);

            _vaultChests[new Tuple<Container, VaultChest>(con, chest)] = con.UpdateCount;
        }

        public override World GetInstance(Client psr)
        {
            var vault = new Vault(false, psr);
            vault.Manager = Manager;
            vault.Init();
            return Manager.AddWorld(vault);
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            foreach (KeyValuePair<Tuple<Container, VaultChest>, int> i in _vaultChests)
            {
                if (i.Key.Item1.UpdateCount > i.Value)
                {
                    try
                    {
                        Manager.Database.DoActionAsync(db =>
                        {
                            i.Key.Item2._Items =
                                Utils.GetCommaSepString(
                                    i.Key.Item1.Inventory.Take(8).Select(_ => _ == null ? -1 : _.ObjectType).ToArray());
                            db.SaveChest(AccountId, i.Key.Item2);
                            _vaultChests[i.Key] = i.Key.Item1.UpdateCount;
                        });
                    }
                    catch (Exception ex)
                    {
                        Program.Services.GetRequiredService<ILogger<Vault>>().LogError(ex,
                            "Error saving vault chest for {AccountId}", AccountId);
                    }
                }
            }
        }

        private class GiftChest
        {
            public List<Item> Items { get; set; }
        }

        public void Reload(Client client)
        {
            psr = client;
            Init();
        }
    }
}
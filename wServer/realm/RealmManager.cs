#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using db;
using db.data;
using db.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RageRealm.Shared.Configuration.WorldServer;
using wServer.logic;
using wServer.networking;
using wServer.realm.commands;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.worlds;
using GameVault = wServer.realm.worlds.Vault;

#endregion

namespace wServer.realm
{
    public class RealmManager : IAsyncDisposable
    {
        public const int MAX_REALM_PLAYERS = 85;

        public static List<string> Realms { get; } = new(44)
        {
            "NexusPortal.Lich", "NexusPortal.Goblin", "NexusPortal.Ghost",
            "NexusPortal.Giant", "NexusPortal.Gorgon", "NexusPortal.Blob",
            "NexusPortal.Leviathan", "NexusPortal.Unicorn", "NexusPortal.Minotaur",
            "NexusPortal.Cube", "NexusPortal.Pirate", "NexusPortal.Spider",
            "NexusPortal.Snake", "NexusPortal.Deathmage", "NexusPortal.Gargoyle",
            "NexusPortal.Scorpion", "NexusPortal.Djinn", "NexusPortal.Phoenix",
            "NexusPortal.Satyr", "NexusPortal.Drake", "NexusPortal.Orc",
            "NexusPortal.Flayer", "NexusPortal.Cyclops", "NexusPortal.Sprite",
            "NexusPortal.Chimera", "NexusPortal.Kraken", "NexusPortal.Hydra",
            "NexusPortal.Slime", "NexusPortal.Ogre", "NexusPortal.Hobbit",
            "NexusPortal.Titan", "NexusPortal.Medusa", "NexusPortal.Golem",
            "NexusPortal.Demon", "NexusPortal.Skeleton", "NexusPortal.Mummy",
            "NexusPortal.Imp", "NexusPortal.Bat", "NexusPortal.Wyrm",
            "NexusPortal.Spectre", "NexusPortal.Reaper", "NexusPortal.Beholder",
            "NexusPortal.Dragon", "NexusPortal.Harpy"
        };

        public ConcurrentDictionary<string, Client> Clients { get; } = new();
        public ConcurrentDictionary<int, World> Worlds { get; } = new();
        public ConcurrentDictionary<string, GuildHall> GuildHalls { get; } = new();
        public ConcurrentDictionary<string, World> LastWorld { get; } = new();
        private readonly ConcurrentDictionary<string, GameVault> _vaults = new();

        private readonly IServiceProvider _services;
        private readonly CancellationTokenSource _cts = new();
        private Task? _logicTask;
        private Task? _networkTask;

        private int _nextClientId;
        private int _nextWorldId;
        public static ConcurrentDictionary<string, byte> CurrentRealmNames = new();

        private ILogger<RealmManager> _logger;

        public RealmManager(ILogger<RealmManager> logger, IOptions<RealmConfiguration> options)
        {
            MaxClients = options.Value.MaxClients;
            TPS = options.Value.Tps;
            _logger = logger;
            Random = new Random();
        }

        public BehaviorDb Behaviors { get; private set; } = null!;
        public ChatManager Chat { get; private set; } = null!;
        public CommandManager Commands { get; private set; } = null!;
        public XmlDataService GameDataService { get; private set; } = null!;
        public LogicTicker Logic { get; private set; } = null!;
        public RealmPortalMonitor Monitor { get; private set; } = null!;
        public NetworkTicker Network { get; private set; } = null!;
        public DatabaseAdapter Database { get; private set; } = null!;
        public Random Random { get; }
        public int MaxClients { get; }
        public int TPS { get; }
        public bool Terminating { get; private set; }

        // -----------------------------------------
        // initialization
        // -----------------------------------------
        public void Initialize()
        {
            _logger.LogInformation("Initializing Realm Manager...");

            GameDataService = Program.Services.GetRequiredService<XmlDataService>();
            Behaviors = new BehaviorDb(this);
            GeneratorCache.Init();
            MerchantLists.InitMerchatLists(GameDataService);

            AddWorld(World.NEXUS_ID, Worlds[0] = new Nexus());
            AddWorld(World.MARKET, new ClothBazaar());
            AddWorld(World.TEST_ID, new Test());
            AddWorld(World.TUT_ID, new Tutorial(true));
            AddWorld(World.DAILY_QUEST_ID, new DailyQuestRoom());

            Monitor = new RealmPortalMonitor(this);

            _ = Task.Run(async () =>
            {
                try
                {
                    var world = await GameWorld.AutoNameAsync(this, 1, true).ConfigureAwait(false);
                    AddWorld(world);
                }
                catch (Exception ex)
                {
                    Program.Logger.LogError(ex, "Failed to auto-name world");
                }
            });


            Chat = new ChatManager(this);
            Commands = new CommandManager(this);

            _logger.LogInformation("Realm Manager initialized.");
        }

        // -----------------------------------------
        // run
        // -----------------------------------------
        public async Task RunAsync()
        {
            _logger.LogInformation("Starting Realm Manager...");

            Network = new NetworkTicker(this);
            Logic = new LogicTicker(this);
            Database = new DatabaseAdapter(_services);

            // run both tickers as background tasks
            _networkTask = Task.Run(() => Network.TickLoop(_cts.Token));
            _logicTask = Task.Run(() => Logic.TickLoop(_cts.Token));

            await Task.CompletedTask;
            _logger.LogInformation("Realm Manager started.");
        }

        public async Task CloseWorldAsync(World world)
        {
            Monitor.WorldRemoved(world);
            await Task.Yield(); // or await cleanup logic
        }

        // -----------------------------------------
        // shutdown
        // -----------------------------------------
        public async Task StopAsync()
        {
            _logger.LogInformation("Stopping Realm Manager...");
            Terminating = true;

            try
            {
                _cts.Cancel();

                // disconnect and save clients
                var clients = Clients.Values.ToList();
                foreach (var c in clients)
                {
                    try
                    {
                        c.Disconnect();
                        await c.Save().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error saving client {AccountName}", c.Account?.Name);
                    }
                }

                GameDataService.Dispose();

                if (_logicTask != null)
                    await _logicTask.ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously)
                        .ConfigureAwait(false);

                if (_networkTask != null)
                    await _networkTask.ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously)
                        .ConfigureAwait(false);

                _logger.LogInformation("Realm Manager stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping Realm Manager");
            }
        }

        // -----------------------------------------
        // connection + client mgmt
        // -----------------------------------------
        public async Task DisconnectAsync(Client client)
        {
            if (client == null) return;
            try
            {
                client.Disconnect();
                await client.Save().ConfigureAwait(false);
                Clients.TryRemove(client.Account.AccountId, out _);
                client.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error disconnecting client");
            }
        }

        public bool TryConnect(Client psr)
        {
            if (Clients.Count >= MaxClients)
            {
                Program.Logger.LogInformation("Kicking {AccountName} because server is full.", psr.Account.Name);
                return false;
            }

            if (psr.Account.Banned)
            {
                Program.Logger.LogInformation("Kicking {AccountName} because account is banned.", psr.Account.Name);
                return false;
            }

            psr.Id = Interlocked.Increment(ref _nextClientId);
            if (Clients.ContainsKey(psr.Account.AccountId) && !Clients[psr.Account.AccountId].Socket.Connected)
            {
                Program.Logger.LogInformation("Removing disconnected client {AccountName}.", psr.Account.Name);
                Clients.TryRemove(psr.Account.AccountId, out _);
            }

            Program.Logger.LogInformation("Client {AccountName} connected.", psr.Account.Name);
            return Clients.TryAdd(psr.Account.AccountId, psr);
        }

        // -----------------------------------------
        // world mgmt
        // -----------------------------------------
        public World AddWorld(int id, World world)
        {
            world.Id = id;
            Worlds[id] = world;
            OnWorldAdded(world);
            return world;
        }

        public World AddWorld(World world)
        {
            world.Id = Interlocked.Increment(ref _nextWorldId);
            Worlds[world.Id] = world;
            OnWorldAdded(world);
            return world;
        }

        public bool RemoveWorld(World world)
        {
            if (world.Manager == null)
                throw new InvalidOperationException("World is not added.");

            if (Worlds.TryRemove(world.Id, out _))
            {
                try
                {
                    OnWorldRemoved(world);
                    world.Dispose();
                    GC.Collect();
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "Error removing world {WorldId} ({WorldName})", world.Id, world.Name);
                }

                return true;
            }

            return false;
        }

        private void OnWorldAdded(World world)
        {
            world.Manager ??= this;
            if (world is GameWorld)
                Monitor.WorldAdded(world);
            _logger.LogInformation("World {WorldId} ({WorldName}) added.", world.Id, world.Name);
        }

        private void OnWorldRemoved(World world)
        {
            world.Manager = null;
            if (world is GameWorld)
                Monitor.WorldRemoved(world);
            _logger.LogInformation("World {WorldId} ({WorldName}) removed.", world.Id, world.Name);
        }

        // -----------------------------------------
        // utility
        // -----------------------------------------
        public GameVault PlayerVault(Client processor)
        {
            if (!_vaults.TryGetValue(processor.Account.AccountId, out var v))
            {
                v = (GameVault)AddWorld(new GameVault(false, processor));
                _vaults.TryAdd(processor.Account.AccountId, v);
            }
            else
                v.Reload(processor);

            return v;
        }

        public bool RemoveVault(string accountId) => _vaults.TryRemove(accountId, out _);

        public Player? FindPlayer(string name)
        {
            if (name.Contains(' '))
                name = name.Split(' ')[1];

            return Worlds.Values
                .Where(w => w.Id != 0)
                .SelectMany(w => w.Players.Values)
                .FirstOrDefault(p =>
                    string.Equals(p.Client.Account.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public Player? FindPlayerRough(string name)
        {
            foreach (var world in Worlds.Values)
            {
                if (world.Id == 0) continue;
                var player = world.GetUniqueNamedPlayerRough(name);
                if (player != null)
                    return player;
            }

            return null;
        }

        public World? GetWorld(int id)
        {
            if (id == 0)
                return null;

            if (Worlds.TryGetValue(id, out var world) && world.Id != 0)
                return world;

            return null;
        }

        public ValueTask DisposeAsync()
        {
            _cts.Cancel();
            _cts.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
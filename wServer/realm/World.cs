#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DungeonGenerator;
using DungeonGenerator.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RageRealm.Shared.Models;
using wServer.networking;
using wServer.networking.svrPackets;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.terrain;
using wServer.realm.worlds;

#endregion

namespace wServer.realm
{
    public abstract class World : IDisposable
    {
        #region Constants

        public const int TUT_ID = -1;
        public const int NEXUS_ID = -2;
        public const int NEXUS_LIMBO = -3;
        public const int VAULT_ID = -5;
        public const int TEST_ID = -6;
        public const int GAUNTLET = -7;
        public const int WC = -8;
        public const int ARENA = -9;
        public const int GHALL = -10;
        public const int MARKET = -11;
        public const int PETYARD_ID = -12;
        public const int DAILY_QUEST_ID = -13;
        public const int GUILD_ID = -14;

        #endregion

        protected readonly ILogger<World> _logger;

        private int _entityInc;
        protected RealmPortalMonitor _portalMonitor;
        protected GeneratorCache _generatorCache;
        private bool _canBeClosed;
        public RealmManager Manager { get; internal set; }

        internal ILogger<World> Logger => _logger;
        internal RealmPortalMonitor PortalMonitor => _portalMonitor;
        internal GeneratorCache GeneratorCache => _generatorCache;

        protected World(RealmManager manager, ILogger<World> logger,
            RealmPortalMonitor portalMonitor, GeneratorCache generatorCache)
        {
            Manager = manager;
            _logger = logger;
            _portalMonitor = portalMonitor;
            _generatorCache = generatorCache;

            Players = new ConcurrentDictionary<int, Player>();
            Enemies = new ConcurrentDictionary<int, Enemy>();
            Quests = new ConcurrentDictionary<int, Enemy>();
            Pets = new ConcurrentDictionary<int, Pet>();
            Projectiles = new ConcurrentDictionary<Tuple<int, byte>, Projectile>();
            StaticObjects = new ConcurrentDictionary<int, StaticObject>();
            Timers = new List<WorldTimer>();
            ClientXml = ExtraXml = Empty<string>.Array;
            AllowTeleport = true;
            ShowDisplays = true;
            MaxPlayers = 85;
            Seed = manager.Random.NextUInt32();
            PortalKey = Utils.RandomBytes(NeedsPortalKey ? 16 : 0);
            
            // mark world for cleanup after 2 minutes if empty
            Timers.Add(new WorldTimer(120 * 1000, (w, t) =>
            {
                _canBeClosed = true;
                if (NeedsPortalKey)
                    PortalKeyExpired = true;
            }));
        }

        public bool IsLimbo { get; protected set; }

        public IEnumerable<Entity> Entities
        {
            get
            {
                foreach (var p in Players.Values)
                    yield return p;
                foreach (var e in Enemies.Values)
                    yield return e;
                foreach (var s in StaticObjects.Values)
                    yield return s;
                foreach (var pr in Projectiles.Values)
                    yield return pr;
                foreach (var pet in Pets.Values)
                    yield return pet;
            }
        }

        public int Id { get; internal set; }
        public int Difficulty { get; protected set; }
        public string Name { get; protected set; }
        public string ClientWorldName { get; protected set; }
        public byte[] PortalKey { get; private set; }
        public bool PortalKeyExpired { get; private set; }
        public uint Seed { get; private set; }
        public virtual bool NeedsPortalKey => false;

        public ConcurrentDictionary<int, Player> Players { get; private set; }
        public ConcurrentDictionary<int, Enemy> Enemies { get; private set; }
        public ConcurrentDictionary<int, Pet> Pets { get; }
        public ConcurrentDictionary<Tuple<int, byte>, Projectile> Projectiles { get; private set; }
        public ConcurrentDictionary<int, StaticObject> StaticObjects { get; private set; }
        public List<WorldTimer> Timers { get; }
        public int Background { get; protected set; }

        public CollisionMap<Entity> EnemiesCollision { get; private set; }
        public CollisionMap<Entity> PlayersCollision { get; private set; }
        public byte[,] Obstacles { get; private set; }

        public bool AllowTeleport { get; protected set; }
        public bool ShowDisplays { get; protected set; }
        public string[] ClientXml { get; protected set; }
        public string[] ExtraXml { get; protected set; }

        public bool Dungeon { get; protected set; }
        public bool Cave { get; protected set; }
        public bool Shaking { get; protected set; }

        public int MaxPlayers { get; protected set; }

        public Wmap Map { get; private set; }
        public ConcurrentDictionary<int, Enemy> Quests { get; }

        public virtual World GetInstance(Client psr) => null;
        public string Uuid { get; set; } = Guid.NewGuid().ToString();

        #region Passability and entity helpers

        public bool IsPassable(int x, int y)
        {
            var tile = Map[x, y];
            if (Manager.GameDataService.Tiles[tile.TileId].NoWalk)
                return false;
            if (Manager.GameDataService.ObjectDescs.TryGetValue(tile.ObjType, out var desc))
            {
                if (!desc.Static)
                    return false;
                if (desc.OccupySquare || desc.EnemyOccupySquare || desc.FullOccupy)
                    return false;
            }

            return true;
        }

        public int GetNextEntityId() => Interlocked.Increment(ref _entityInc);

        #endregion

        #region Initialization

        protected virtual void Init()
        {
            // spawn async init so Manager assignment doesn't block
            _ = InitAsync();
        }

        public virtual async Task InitAsync() => await Task.CompletedTask;

        protected async Task FromWorldMapAsync(Stream dat)
        {
            await Task.Run(() => FromWorldMap(dat)).ConfigureAwait(false);
        }

        public Player? GetUniqueNamedPlayerRough(string name)
        {
            foreach (var player in Players.Values)
            {
                if (player.CompareName(name))
                    return player;
            }

            return null;
        }

        public Entity? GetEntity(int id)
        {
            if (Players.TryGetValue(id, out var player))
                return player;
            if (Enemies.TryGetValue(id, out var enemy))
                return enemy;
            if (StaticObjects.TryGetValue(id, out var staticObj))
                return staticObj;
            if (Projectiles.Any(k => k.Key.Item1 == id) &&
                Projectiles.TryGetValue(
                    Projectiles.Keys.FirstOrDefault(k => k.Item1 == id), out var projectile))
                return projectile;
            return Pets.GetValueOrDefault(id);
        }

        public bool IsFull => Players.Count >= MaxPlayers;

        /// <summary>
        /// Broadcast a packet to all players in the world (optionally nearby the source entity)
        /// </summary>
        public void BroadcastPacket(Packet pkt, Entity? entity = null)
        {
            foreach (var p in Players.Values)
            {
                if (entity == null || p.DistSqr(entity) < Player.SIGHTRADIUS)
                    p.Client.SendPacket(pkt);
            }
        }

        public void BroadcastPackets(IEnumerable<Packet> packets, Entity? entity = null)
        {
            foreach (var packet in packets)
                BroadcastPacket(packet, entity);
        }

        public void BroadcastPacketSync(Packet pkt, Entity? entity = null)
        {
            // same as BroadcastPacket, but no async dispatch — used in chat
            foreach (var p in Players.Values)
            {
                if (entity == null || p.DistSqr(entity) < Player.SIGHTRADIUS)
                    p.Client.SendPacket(pkt);
            }
        }

        public Player? GetPlayerByName(string name)
        {
            return Players.Values.FirstOrDefault(p =>
                string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }


        public void ChatReceived(Player from, string text)
        {
            var packet = new TextPacket
            {
                BubbleTime = 0,
                Stars = from.Stars,
                Name = from.Name,
                ObjectId = from.Id,
                Recipient = "",
                Text = text,
                CleanText = text
            };

            BroadcastPacket(packet, from);
        }


        private void FromWorldMap(Stream dat)
        {
            var logger = Program.Services?.GetRequiredService<ILogger<Wmap>>();
            var map = new Wmap(Manager.GameDataService, logger);
            Map = map;
            _entityInc = 0;
            _entityInc += Map.Load(dat, 0);

            int w = Map.Width, h = Map.Height;
            Obstacles = new byte[w, h];

            for (var y = 0; y < h; y++)
            for (var x = 0; x < w; x++)
            {
                try
                {
                    var tile = Map[x, y];
                    if (Manager.GameDataService.Tiles[tile.TileId].NoWalk)
                        Obstacles[x, y] = 3;
                    if (Manager.GameDataService.ObjectDescs.TryGetValue(tile.ObjType, out var desc))
                    {
                        if (desc.Class == "Wall" || desc.Class == "ConnectedWall" || desc.Class == "CaveWall")
                            Obstacles[x, y] = 2;
                        else if (desc.OccupySquare || desc.EnemyOccupySquare)
                            Obstacles[x, y] = 1;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing tile at ({X}, {Y})", x, y);
                }
            }

            EnemiesCollision = new CollisionMap<Entity>(0, w, h);
            PlayersCollision = new CollisionMap<Entity>(1, w, h);

            Projectiles.Clear();
            StaticObjects.Clear();
            Enemies.Clear();
            Players.Clear();

            foreach (var entity in Map.InstantiateEntities(Manager))
            {
                if (entity.ObjectDesc != null &&
                    (entity.ObjectDesc.OccupySquare || entity.ObjectDesc.EnemyOccupySquare))
                    Obstacles[(int)(entity.X - 0.5), (int)(entity.Y - 0.5)] = 2;
                EnterWorld(entity);
            }
        }

        protected async Task LoadMapAsync(string embeddedResource, MapType type)
        {
            if (embeddedResource == null)
                return;

            var stream = typeof(RealmManager).Assembly.GetManifestResourceStream(embeddedResource);
            if (stream == null)
                throw new ArgumentException("Resource not found", nameof(embeddedResource));

            switch (type)
            {
                case MapType.Wmap:
                    await FromWorldMapAsync(stream);
                    break;

                case MapType.Json:
                    using (var reader = new StreamReader(stream))
                    {
                        var json = await reader.ReadToEndAsync();
                        var converted = Json2Wmap.Convert(Manager, json);
                        await FromWorldMapAsync(new MemoryStream(converted));
                    }

                    break;

                default:
                    throw new ArgumentException("Invalid MapType");
            }
        }

        protected async Task LoadMapAsync(string json)
        {
            var converted = await Task.Run(() => Json2Wmap.Convert(Manager, json));
            await FromWorldMapAsync(new MemoryStream(converted));
        }

        // Synchronous methods for backward compatibility
        protected void LoadMap(string embeddedResource, MapType type)
        {
            LoadMapAsync(embeddedResource, type).GetAwaiter().GetResult();
        }

        protected void LoadMap(string json)
        {
            LoadMapAsync(json).GetAwaiter().GetResult();
        }

        protected void LoadMap(byte[] worldMapData)
        {
            FromWorldMap(new MemoryStream(worldMapData));
        }

        #endregion

        #region Tick Logic (unchanged)

        public virtual void Tick(RealmTime time)
        {
            try
            {
                if (IsLimbo)
                    return;

                for (int i = 0; i < Timers.Count; i++)
                {
                    try
                    {
                        if (Timers[i] == null) continue;
                        if (!Timers[i].Tick(this, time)) continue;
                        Timers.RemoveAt(i);
                        i--;
                    }
                    catch
                    {
                        /* ignored */
                    }
                }

                foreach (var p in Players.Values)
                {
                    p?.Tick(time);
                }

                foreach (var pet in Pets.Values) pet.Tick(time);

                if (EnemiesCollision != null)
                {
                    foreach (var e in EnemiesCollision.GetActiveChunks(PlayersCollision))
                        e.Tick(time);
                    foreach (var decoy in StaticObjects.Values.Where(x => x is Decoy))
                        decoy.Tick(time);
                }
                else
                {
                    foreach (var e in Enemies.Values) e.Tick(time);
                    foreach (var s in StaticObjects.Values) s.Tick(time);
                }

                foreach (var proj in Projectiles.Values)
                    proj.Tick(time);

                if (Players.Count == 0 && _canBeClosed && IsDungeon())
                {
                    if (this is Vault v)
                        Manager.RemoveVault(v.AccountId);
                    Manager.RemoveWorld(this);
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Error in world {WorldName}", Name);
            }
        }

        public bool IsDungeon()
        {
            return !(this is Nexus)
                   && !(this is GameWorld)
                   && !(this is ClothBazaar)
                   && !(this is Test)
                   && !(this is GuildHall)
                   && !(this is Tutorial)
                   && !(this is DailyQuestRoom)
                   && !IsLimbo;
        }

        #endregion

        #region Misc (EnterWorld, LeaveWorld, etc.)

        public virtual int EnterWorld(Entity entity)
        {
            switch (entity)
            {
                case Player player:
                    try
                    {
                        player.Id = GetNextEntityId();
                        entity.Init(this);
                        Players.TryAdd(player.Id, player);
                        PlayersCollision.Insert(player);
                    }
                    catch (Exception e)
                    {
                        _logger?.LogError(e, "Error adding player {PlayerName} to world {WorldName}", player.Name,
                            Name);
                    }

                    break;
                case Enemy enemy:
                {
                    enemy.Id = GetNextEntityId();
                    entity.Init(this);
                    Enemies.TryAdd(enemy.Id, enemy);
                    EnemiesCollision.Insert(enemy);
                    if (enemy.ObjectDesc.Quest)
                        Quests.TryAdd(enemy.Id, enemy);
                    break;
                }
                case Projectile projectile:
                {
                    projectile.Init(this);
                    var prj = projectile;
                    Projectiles[new Tuple<int, byte>(prj.ProjectileOwner.Self.Id, prj.ProjectileId)] = prj;
                    break;
                }
                case StaticObject staticObject:
                {
                    staticObject.Id = GetNextEntityId();
                    staticObject.Init(this);
                    StaticObjects.TryAdd(staticObject.Id, staticObject);
                    if (entity is Decoy)
                        PlayersCollision.Insert(staticObject);
                    else
                        EnemiesCollision.Insert(staticObject);
                    break;
                }
                default:
                {
                    var pet = entity as Pet;
                    if (pet == null) return entity.Id;
                    if (pet.IsPet)
                    {
                        pet.Id = GetNextEntityId();
                        pet.Init(this);
                        if (!Pets.TryAdd(pet.Id, pet))
                            _logger?.LogError("Failed to add pet {PetName} to world {WorldName}", pet.Name, Name);

                        PlayersCollision.Insert(pet);
                    }
                    else
                        _logger?.LogWarning("This is not a real pet! {PetName}", pet.Name);

                    break;
                }
            }

            return entity.Id;
        }

        public virtual void LeaveWorld(Entity entity)
        {
            switch (entity)
            {
                case Player player:
                    if (!Players.TryRemove(player.Id, out _))
                        _logger?.LogWarning("Could not remove player {PlayerName} from world {WorldName}", player.Name,
                            Name);
                    PlayersCollision.Remove(player);
                    break;

                case Enemy enemy:
                    Enemies.TryRemove(enemy.Id, out _);
                    EnemiesCollision.Remove(enemy);
                    if (enemy.ObjectDesc.Quest)
                        Quests.TryRemove(enemy.Id, out _);
                    break;

                case Projectile projectile:
                    Projectiles.TryRemove(
                        new Tuple<int, byte>(projectile.ProjectileOwner.Self.Id, projectile.ProjectileId), out _);
                    break;

                case StaticObject staticObj:
                    StaticObjects.TryRemove(staticObj.Id, out _);
                    if (staticObj is Decoy)
                        PlayersCollision.Remove(staticObj);
                    else
                        EnemiesCollision.Remove(staticObj);
                    break;

                case Pet pet when pet.IsPet:
                    Pets.TryRemove(pet.Id, out _);
                    PlayersCollision.Remove(pet);
                    break;

                default:
                    break;
            }

            entity.Owner = null;
            entity.Dispose();
        }


        public virtual void Dispose()
        {
            Map?.Dispose();
            Players.Clear();
            Enemies.Clear();
            Quests.Clear();
            Pets.Clear();
            Projectiles.Clear();
            StaticObjects.Clear();
            Timers.Clear();
            EnemiesCollision = null;
            PlayersCollision = null;
        }

        #endregion
    }

    public enum MapType
    {
        Wmap,
        Json
    }
}
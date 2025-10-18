#region

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RageRealm.Shared.Models;
using wServer.Factories;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.setpieces;

#endregion

namespace wServer.realm.worlds
{
    public class GameWorld : World
    {
        public ILogger<GameWorld> Logger;

        private readonly int _mapId;
        private readonly bool _oryxPresent;
        private string _displayName;

        public Oryx? Overseer { get; private set; }

        public GameWorld(int mapId, string name, bool oryxPresent, ILogger<GameWorld> logger, RealmManager? manager = null) : base(manager)
        {
            _displayName = name;
            Name = name;
            ClientWorldName = name;
            Background = 0;
            Difficulty = -1;
            _oryxPresent = oryxPresent;
            _mapId = mapId;
            Logger = logger;
        }

        /// <summary>
        /// Asynchronous world initialization.
        /// </summary>
        public async Task InitAsync()
        {
            Logger.LogInformation("Initializing Game World {ID} ({Name}) from map {MapId}...", Id, Name, _mapId);

            // just await directly — LoadMapAsync is already asynchronous
            await LoadMapAsync($"wServer.realm.worlds.maps.world{_mapId}.wmap", MapType.Wmap)
                .ConfigureAwait(false);

            // only wrap CPU-heavy synchronous work (like setpiece placement)
            await Task.Run(() => SetPieces.ApplySetPieces(this))
                .ConfigureAwait(false);

            Overseer = _oryxPresent ? new Oryx(this) : null;

            Logger.LogInformation("Game World initialized.");
        }


        /// <summary>
        /// Asynchronously creates a new GameWorld with an automatically selected name.
        /// </summary>
        public static async Task<GameWorld> AutoNameAsync(int mapId, bool oryxPresent)
        {
            // Pick a random available realm name
            string name = RealmManager.Realms[new Random().Next(RealmManager.Realms.Count)];

            // Track the name to prevent reuse
            RealmManager.Realms.Remove(name);
            RealmManager.CurrentRealmNames.TryAdd(name, 0);

            var gameWorldFactory = Program.Services.GetRequiredService<IGameWorldFactory>();
            
            // Create and initialize the world asynchronously
            var world = gameWorldFactory.Create(mapId, name, oryxPresent);
            
            await world.InitAsync();
            return world;
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);
            Overseer?.Tick(time);
        }

        public void EnemyKilled(Enemy enemy, Player killer)
        {
            Overseer?.OnEnemyKilled(enemy, killer);
        }

        public override int EnterWorld(Entity entity)
        {
            int ret = base.EnterWorld(entity);
            if (entity is Player p)
                Overseer?.OnPlayerEntered(p);
            return ret;
        }

        public override void Dispose()
        {
            Overseer?.Dispose();
            base.Dispose();
        }
    }
}
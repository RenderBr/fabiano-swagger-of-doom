#region

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using wServer.Events;
using wServer.Factories;
using wServer.Models.Properties;
using wServer.realm.entities;
using wServer.realm.worlds;

#endregion

namespace wServer.realm
{
    public class RealmPortalMonitor
    {
        private readonly ILogger _logger;
    private Nexus nexus;
        private readonly Random rand = new Random();
        private readonly object worldLock = new object();
        public Dictionary<World, Portal> portals = new Dictionary<World, Portal>();
        private readonly IPortalFactory portalFactory;

        public RealmPortalMonitor(ILogger<RealmPortalMonitor> logger, IEventBus _bus, IPortalFactory portalFactory)
        {
            _logger = logger;
            this.portalFactory = portalFactory;
            _logger?.LogInformation("Initializing Portal Monitor...");

            _bus.Subscribe<WorldEvents.WorldCreatedEvent>(WorldAdded);
            _bus.Subscribe<WorldEvents.WorldClosedEvent>(WorldClosed);
        }

        private Position GetRandPosition()
        {
            int x, y;
            do
            {
                x = rand.Next(0, nexus.Map.Width);
                y = rand.Next(0, nexus.Map.Height);
            } while (
                portals.Values.Any(_ => _.X == x && _.Y == y) ||
                nexus.Map[x, y].Region != TileRegion.Realm_Portals);

            return new Position { X = x, Y = y };
        }

        public void WorldAdded(WorldEvents.WorldCreatedEvent evt)
        {
            lock (worldLock)
            {
                if (evt.World is Nexus nx)
                {
                    nexus = nx;
                }

                if (nexus == null)
                {
                    _logger?.LogWarning("RealmPortalMonitor: Nexus not initialized yet; skipping portal creation for world {WorldId} ({WorldName}).", evt.World.Id, evt.World.Name);
                    return;
                }

                var pos = GetRandPosition();
                var portal = portalFactory.CreatePortal(new CreatePortalProperties()
                {
                    ObjType = 0x0712,
                    Life = null,
                    Name = evt.World.Name,
                    WorldInstance = evt.World,
                    Size = 80
                }).Result;

                portal.Move(pos.X + 0.5f, pos.Y + 0.5f);
                nexus.EnterWorld(portal);
                portals.Add(evt.World, portal);
                _logger?.LogInformation("World {WorldId}({WorldName}) added to monitor.", evt.World.Id, evt.World.Name);
            }
        }

        public void WorldRemoved(World world)
        {
            lock (worldLock)
            {
                if (portals.ContainsKey(world))
                {
                    Portal portal = portals[world];
                    nexus.LeaveWorld(portal);
                    RealmManager.Realms.Add(portal.PortalName);
                    RealmManager.CurrentRealmNames.TryRemove(portal.PortalName, out _);
                    portals.Remove(world);
                    _logger?.LogInformation("World {WorldId}({WorldName}) removed from monitor.", world.Id, world.Name);
                }
            }
        }

        public void WorldClosed(WorldEvents.WorldClosedEvent evt)
        {
            var world = evt.World;

            lock (worldLock)
            {
                Portal portal = portals[world];
                nexus.LeaveWorld(portal);
                portals.Remove(world);
                _logger?.LogInformation("World {WorldId}({WorldName}) closed.", world.Id, world.Name);
            }
        }

        public void WorldOpened(World world)
        {
            lock (worldLock)
            {
                if (nexus == null)
                {
                    _logger?.LogWarning("RealmPortalMonitor: Nexus not initialized yet; skipping portal creation for world {WorldId} ({WorldName}).", world.Id, world.Name);
                    return;
                }
                var pos = GetRandPosition();
                var portal = portalFactory.CreatePortal(new CreatePortalProperties()
                {
                    ObjType = 0x0712,
                    Life = null,
                    Name = world.Name,
                    WorldInstance = world,
                    Size = 80
                }).Result;
                portal.Move(pos.X, pos.Y);
                nexus.EnterWorld(portal);
                portals.Add(world, portal);
                _logger?.LogInformation("World {WorldId}({WorldName}) opened.", world.Id, world.Name);
            }
        }
    }
}
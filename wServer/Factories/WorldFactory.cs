using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using wServer.realm;

namespace wServer.Factories;

public class WorldFactory(
    RealmManager manager,
    ILogger<World> logger,
    RealmPortalMonitor portalMonitor,
    GeneratorCache generatorCache)
    : IWorldFactory
{
    public async Task<World> CreateWorldAsync(Type worldType)
    {
        if (!typeof(World).IsAssignableFrom(worldType))
            throw new ArgumentException("worldType must be a subclass of World", nameof(worldType));

        try
        {
            var world = (World)Activator.CreateInstance(worldType, manager, logger, portalMonitor, generatorCache);
            if (world == null)
                throw new InvalidOperationException($"Could not create an instance of {worldType.FullName}");

            await world.InitAsync().ConfigureAwait(false);
            await manager.AddWorldAsync(world);
            return world;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create world of type {WorldType}", worldType.FullName);
            throw;
        }
    }
}
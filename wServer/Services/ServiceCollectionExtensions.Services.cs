using db.Services;
using Microsoft.Extensions.DependencyInjection;
using wServer.Factories;
using wServer.networking;
using wServer.realm;
using wServer.realm.commands;

namespace wServer.Services;

public static class ServiceCollectionExtensions_Services
{
    public static IServiceCollection AddHandlingServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(Command))
            .AddClasses(classes => classes.AssignableTo<Command>(), publicOnly: false)
            .AsSelf()
            .WithTransientLifetime());
        
        services.AddSingleton<CommandManager>();
        services.AddSingleton<GeneratorCache>()
            .AddSingleton<INetworkHandlerFactory, NetworkHandlerFactory>()
            .AddSingleton<IClientFactory, ClientFactory>()
            .AddSingleton<IPacketHandlerFactory, PacketHandlerFactory>()
            .AddSingleton<PacketHandlers>()
            .AddSingleton<RealmPortalMonitor>()
            .AddSingleton<IPortalFactory, PortalFactory>()
            .AddSingleton<IGameWorldFactory, GameWorldFactory>()
            .AddSingleton<IWorldFactory, WorldFactory>()
            .AddSingleton<CharacterCreationService>()
            .AddSingleton<RealmManager>();
        return services;
    }
}
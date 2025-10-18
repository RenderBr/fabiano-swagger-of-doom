using Microsoft.Extensions.DependencyInjection;
using wServer.realm.commands;

namespace wServer.Services;

public static class ServiceCollectionExtensions_Services
{
    public static IServiceCollection AddHandlingServices(this IServiceCollection services)
    {
        services.AddSingleton<GeneratorCache>();
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(Command))
            .AddClasses(classes => classes.AssignableTo<Command>(), publicOnly: false)
            .AsSelf()
            .WithTransientLifetime());
        services.AddSingleton<CommandManager>();
        return services;
    }
}
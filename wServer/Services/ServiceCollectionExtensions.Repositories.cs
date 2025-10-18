using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

namespace wServer.Services;

public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddScoped<IDailyQuestRepository, DailyQuestRepository>();
        services.AddScoped<IDeathRepository, DeathRepository>();
        services.AddScoped<IGuildRepository, GuildRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IVaultRepository, VaultRepository>();
        services.AddScoped<IStatRepository, StatRepository>();
        services.AddScoped<AccountService>();

        return services;
    }
}
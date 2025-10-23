using db;
using db.data;
using db.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RageRealm.Shared.Configuration.WorldServer;
using wServer.realm;

namespace wServer.Services;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration config)
    {
        var configuration = config.Get<WorldServerConfiguration>();

        var connString =
            $"server={configuration.Database.Host};userid={configuration.Database.User};password={configuration.Database.Password};" +
            $"database={configuration.Database.Name};AllowPublicKeyRetrieval=True;SslMode=none;Convert Zero Datetime=True;";

        services.AddDbContext<ServerDbContext>(options =>
        {
            options.UseMySql(connString, ServerVersion.AutoDetect(connString));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<DatabaseAdapter>();
        return services;
    }
}
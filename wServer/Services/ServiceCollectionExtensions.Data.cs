using db;
using db.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RageRealm.Shared.Configuration.WorldServer;
using wServer.realm;

namespace wServer.Services;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration _config)
    {
        var config = _config.Get<WorldServerConfiguration>();
        
        var connString =
            $"server={config.Database.Host};userid={config.Database.User};password={config.Database.Password};" +
            $"database={config.Database.Name};AllowPublicKeyRetrieval=True;SslMode=none;Convert Zero Datetime=True;";

        services.AddDbContext<ServerDbContext>(options =>
        {
            options.UseMySql(connString, ServerVersion.AutoDetect(connString));
        });
        
        services.AddSingleton<DatabaseAdapter>();
        services.AddSingleton<GeneratorCache>();

        return services;
    }

}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Profiles.Application.Data;
using Profiles.Infrastructure.Data;
using StackExchange.Redis;

namespace Profiles.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration config)
    {
        var connectionString = config.GetConnectionString("PostgresConnection");

        services.AddDbContext<ProfilesContext>(options =>
        {
            var conn = new NpgsqlConnectionStringBuilder(connectionString)
            {
                TrustServerCertificate = true,
            };
            options.UseNpgsql(conn.ConnectionString);
        });
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(config.GetConnectionString("RedisConnection")!);
        });


        services.AddScoped<IProfilesRepository, ProfilesRepository>();
        services.AddScoped<IProfilesRedisRepository, ProfilesRedisRepository>();

        return services;
    }
}
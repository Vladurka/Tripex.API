using Cassandra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Posts.Application.Data;
using Posts.Infrastructure.Data;
using StackExchange.Redis;

namespace Posts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<ScyllaDbSettings>(
            config.GetSection("ConnectionStrings:ScyllaDb"));

        services.AddSingleton<ISession>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScyllaDbSettings>>().Value;

            var clusterBuilder = Cluster.Builder()
                .AddContactPoints(settings.ContactPoints);

            if (!string.IsNullOrEmpty(settings.Username))
                clusterBuilder = clusterBuilder.WithCredentials(settings.Username, settings.Password);

            return clusterBuilder.Build().Connect(settings.Keyspace);
        });

        services.AddScoped<IPostRepository, PostRepository>();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(config.GetConnectionString("RedisConnection")!);
        });

        services.AddScoped<IPostsRedisRepository, PostsRedisRepository>();

        return services;
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        
        services.AddScoped<IPostRepository, PostRepository>();
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(config.GetConnectionString("RedisConnection")!);
        });

        services.AddScoped<IPostsRedisRepository, PostsRedisRepository>();
        
        return services;
    }
}

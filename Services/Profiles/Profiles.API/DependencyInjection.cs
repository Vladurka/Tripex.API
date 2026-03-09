using BuildingBlocks.Auth;
using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Messaging.Outbox;
using Profiles.Infrastructure.Data;

namespace Profiles.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddHealthChecks();
        services.AddOutboxPattern<ProfilesContext>();
        services.AddUserContext();
        
        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.MapCarter();
        app.UseExceptionHandler(opts => { });
        app.UseHealthChecks("/health");

        return app;
    }
}
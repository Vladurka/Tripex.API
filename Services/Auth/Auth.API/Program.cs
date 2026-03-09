using System.Reflection;
using Auth.API.Data;
using Auth.API.Services;
using BuildingBlocks.Auth;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Messaging.MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddDbContext<AuthContext>(opt =>
{
    var originConn = builder.Configuration.GetConnectionString("PostgresConnection");

    var conn = new NpgsqlConnectionStringBuilder(originConn)
    {
        TrustServerCertificate = true
    };

    opt.UseNpgsql(conn.ConnectionString);
});


builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICookiesService, CookiesService>();
builder.Services.AddScoped<IUsersRepository, UserRepository>();

builder.Services.AddOutboxPattern<AuthContext>();

builder.Services.AddCarter();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddHealthChecks();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());

builder.Services.AddUserContext();

var app = builder.Build();

app.MapCarter();
app.UseExceptionHandler(opts => { });
app.UseHealthChecks("/health");

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();
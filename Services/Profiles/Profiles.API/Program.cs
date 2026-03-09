using Microsoft.EntityFrameworkCore;
using Profiles.API;
using Profiles.Application;
using Profiles.Infrastructure;
using Profiles.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProfilesContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseApiServices();

app.Run();
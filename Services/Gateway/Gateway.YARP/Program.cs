using Microsoft.AspNetCore.RateLimiting;
using BuildingBlocks.Auth;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthRequired", p => p.RequireAuthenticatedUser());
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(context =>
    {
        context.AddRequestTransform(transform =>
        {
            transform.ProxyRequest.Headers.Remove("X-User-Id");

            var user = transform.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst("userId")?.Value;
                if (!string.IsNullOrEmpty(userId))
                    transform.ProxyRequest.Headers.Add("X-User-Id", userId);
            }

            return ValueTask.CompletedTask;
        });
    });

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("RateLimiting", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 10;
        opt.QueueLimit = 0;
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseRateLimiter();

app.UseAuth(); 

app.MapReverseProxy()
    .RequireRateLimiting("RateLimiting");

app.Run();
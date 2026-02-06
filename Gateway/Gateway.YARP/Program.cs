using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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

app.UseRateLimiter();
app.MapReverseProxy()
    .RequireRateLimiting("RateLimiting");

app.UseCors("AllowFrontend");

app.Run();
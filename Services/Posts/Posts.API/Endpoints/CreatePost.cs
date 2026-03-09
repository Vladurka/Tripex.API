using System.Text.Json;
using Posts.Application.Posts.Commands.CreatePost;
using StackExchange.Redis;

namespace Posts.API.Endpoints;

public class CreatePost : ICarterModule
{
    private static readonly TimeSpan IdempotencyTtl = TimeSpan.FromMinutes(10);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/posts",
                async ([FromForm] CreatePostCommand command, [FromServices] ISender sender,
                    [FromServices] IJwtHelper helper, [FromServices] IConnectionMultiplexer redis,
                    HttpContext httpContext) =>
        {
            command.ProfileId = helper.GetUserIdByToken();

            var idempotencyKey = httpContext.Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                var db = redis.GetDatabase();
                var cacheKey = $"idempotency:{idempotencyKey}";
                var cached = await db.StringGetAsync(cacheKey);

                if (!cached.IsNullOrEmpty)
                    return Results.Ok(JsonSerializer.Deserialize<CreatePostResult>(cached!));

                var result = await sender.Send(command);
                await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(result), IdempotencyTtl);
                return Results.Created("api/posts", result);
            }

            var res = await sender.Send(command);
            return Results.Created("api/posts", res);
        })
        .DisableAntiforgery()
        .WithName("CreatePost")
        .Produces<CreatePostResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithSummary("Create post")
        .WithDescription("Create post");
    }
}
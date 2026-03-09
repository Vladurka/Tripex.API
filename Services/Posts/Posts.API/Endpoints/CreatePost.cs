using Posts.Application.Posts.Commands.CreatePost;

namespace Posts.API.Endpoints;

public class CreatePost : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/posts", 
                async ([FromForm] CreatePostCommand command, [FromServices] ISender sender, [FromServices] IJwtHelper helper) =>
        {
            command.ProfileId = helper.GetUserIdByToken();
            var result = await sender.Send(command);
            return Results.Created($"api/posts", result);
        })
        .DisableAntiforgery()
        .WithName("CreatePost")
        .Produces<CreatePostResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Create post")
        .WithDescription("Create post");
    }
}
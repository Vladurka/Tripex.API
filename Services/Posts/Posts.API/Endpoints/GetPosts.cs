using Posts.Application.Posts.Queries.GetPosts;

namespace Posts.API.Endpoints;

public class GetPosts : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/posts", async ([FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetPostsQuery());
            return Results.Ok(result);
        })
        .WithName("GetPosts")
        .Produces<GetPostsResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get posts")
        .WithDescription("Get posts");
    }
}
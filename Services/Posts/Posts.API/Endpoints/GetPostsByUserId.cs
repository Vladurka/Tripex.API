using Posts.Application.Posts.Queries.GetPosts;
using Posts.Application.Posts.Queries.GetPostsByUser;

namespace Posts.API.Endpoints;

public class GetPostsByUserId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/posts/user/{userId:guid}", 
                async (Guid userId, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new GetPostsByUserQuery(userId));
                return Results.Ok(result);
            })
            .WithName("GetPostsByUserId")
            .Produces<GetPostsResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get posts by user id")
            .WithDescription("Get posts by user id");
    }
}
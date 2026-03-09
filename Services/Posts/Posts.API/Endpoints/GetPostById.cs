using Posts.Application.Posts.Commands.DeletePost;
using Posts.Application.Posts.Queries.GetPostById;

namespace Posts.API.Endpoints;

public class GetPostById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/posts/{postId:guid}", async (Guid postId, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new GetPostByIdQuery(postId));
                return Results.Ok(result);
            })
            .WithName("GetPostById")
            .Produces<DeletePostResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get post by id")
            .WithDescription("Get post by id");
    }
}
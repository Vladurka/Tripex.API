using Posts.Application.Posts.Commands.DeletePost;

namespace Posts.API.Endpoints;

public class DeletePost : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/posts/{postId:guid}", 
                async (Guid postId, [FromServices] ISender sender, [FromServices] IJwtHelper helper) =>
        { 
            var result = await sender.Send(
                new DeletePostCommand() 
                    { 
                        PostId = postId, 
                        ProfileId = helper.GetUserIdByToken()
                    });
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithName("DeletePost")
        .Produces<DeletePostResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete post")
        .WithDescription("Delete post");
    }
}
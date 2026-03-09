using BuildingBlocks.Pagination;
using Posts.Application.Posts.Queries.GetPosts;

namespace Posts.API.Endpoints;

public class GetPosts : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/posts",
            async ([AsParameters] PaginationRequest pagination, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetPostsQuery(pagination));
            return Results.Ok(result);
        })
        .WithName("GetPosts")
        .Produces<GetPostsResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get posts")
        .WithDescription("Get posts with pagination");
    }
}
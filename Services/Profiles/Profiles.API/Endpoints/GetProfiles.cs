using BuildingBlocks.Pagination;
using Profiles.Application.Profiles.Queries.GetProfiles;

namespace Profiles.API.Endpoints;

public class GetProfiles : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/profiles",
            async ([AsParameters] PaginationRequest pagination, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetProfilesQuery(pagination));
            return Results.Ok(result);
        })
        .WithName("GetProfiles")
        .Produces<GetProfilesResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get profiles")
        .WithDescription("Get profiles with pagination");
    }
}
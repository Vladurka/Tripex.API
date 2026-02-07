using Profiles.Application.Profiles.Queries.GetProfiles;

namespace Profiles.API.Endpoints;

public class GetProfiles : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/profiles", async ([FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetProfilesQuery());
            return Results.Ok(result);
        })
        .WithName("GetProfiles")
        .Produces<GetProfilesQuery>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get profiles")
        .WithDescription("Get profiles");
    }
}
using Profiles.Application.Profiles.Queries.SearchProfilesByName;

namespace Profiles.API.Endpoints;

public class GetProfileByName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/profiles/{name}", async (string name, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new SearchProfilesByNameQuery(name));
            return Results.Ok(result);
        })
        .WithName("SearchProfilesByName")
        .Produces<SearchProfilesByNameQuery>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get profile")
        .WithDescription("Get profile by name");
    }
}
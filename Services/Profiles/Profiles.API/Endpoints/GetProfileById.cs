using Profiles.Application.Profiles.Queries.GetProfileById;

namespace Profiles.API.Endpoints;

public class GetProfileById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/profiles/{id:guid}", async (Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetProfileByIdQuery(id));
            return Results.Ok(result);
        })
        .AllowAnonymous()
        .WithName("GetProfileById")
        .Produces<GetProfileByIdQuery>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get profile")
        .WithDescription("Get profile by id");
    }
}
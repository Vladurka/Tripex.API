using Profiles.Application.Profiles.Queries.GetBasicInfo;

namespace Profiles.API.Endpoints;

public class GetBasicInfo : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/profiles/basic/{id:guid}", async (Guid id, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new GetBaseInfoQuery(id));
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("GetBaseInfo")
            .Produces<GetBaseInfoQuery>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get base info")
            .WithDescription("Get base info");
    }
}
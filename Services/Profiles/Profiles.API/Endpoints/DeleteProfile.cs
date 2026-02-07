using Profiles.Application.Profiles.Commands.DeleteProfile;

namespace Profiles.API.Endpoints;

public class DeleteProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/profiles", async ([FromServices] ISender sender, [FromServices] IJwtHelper helper) =>
            {
                var result = await sender.Send(new DeleteProfileCommand(helper.GetUserIdByToken()));
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("DeleteProfile")
            .Produces<DeleteProfileResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete profile")
            .WithDescription("Delete profile");
    }
}
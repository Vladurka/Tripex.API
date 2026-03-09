using Profiles.Application.Profiles.Commands.UpdateProfile;
using Profiles.Application.Profiles.Queries;

namespace Profiles.API.Endpoints;

public class UpdateProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/profiles", 
                async (UpdateProfileCommand command, [FromServices] ISender sender, [FromServices] IJwtHelper helper) =>
        {
            command.ProfileId = helper.GetUserIdByToken();
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateProfile")
        .Produces<GetProfileResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update profile")
        .WithDescription("Update profile");
    }
}
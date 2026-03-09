using Microsoft.AspNetCore.Mvc;
using Profiles.Application.Profiles.Commands.UpdateAvatar;

namespace Profiles.API.Endpoints;

public class UpdateAvatar : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/profiles/avatar",
                async ([FromForm] UpdateAvatarCommand command, [FromServices] ISender sender, [FromServices] IJwtHelper helper) =>
        {
            command.ProfileId = helper.GetUserIdByToken();
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .DisableAntiforgery() 
        .WithName("UpdateAvatar")
        .Produces<UpdateAvatarResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update avatar")
        .WithDescription("Update avatar");
    }
}
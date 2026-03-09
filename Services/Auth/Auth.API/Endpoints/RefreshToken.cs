using Auth.API.Auth.Commands.RefreshToken;

namespace Auth.API.Endpoints;

public class RefreshToken : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/refresh", async ([FromBody] RefreshTokenCommand command, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("RefreshToken")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("RefreshToken")
            .WithDescription("Refresh access token using a valid refresh token.");
    }
}
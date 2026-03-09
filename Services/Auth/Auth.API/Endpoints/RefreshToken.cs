using Auth.API.Auth.Commands.RefreshToken;

namespace Auth.API.Endpoints;

public class RefreshToken : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/refresh/{token}", async (string token, [FromServices] ISender sender) =>{
            {
                await sender.Send(new RefreshTokenCommand(token));
                return Results.Ok(true);
            }})
            .WithName("RefreshToken")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("RefreshToken")
            .WithDescription("RefreshToken");
    }
}
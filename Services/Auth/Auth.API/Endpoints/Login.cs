using Auth.API.Auth.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Endpoints;

public class Login : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/login",
                async (LoginCommand command, [FromServices] ISender sender) =>
                {
                    var result = await sender.Send(command);
                    return Results.Ok(result);
                })
            .WithName("Login")
            .Produces<LoginResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Login")
            .WithDescription("Login");
    }
}
using Auth.API.Auth.Commands.Signup;

namespace Auth.API.Endpoints;

public class Signup : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/signup", async (SignupCommand command, [FromServices] ISender sender) =>{
            {
                var result = await sender.Send(command);
                return Results.Ok(result);
            }})
            .WithName("Signup")
            .Produces<RegisterResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Signup")
            .WithDescription("Signup");
    }
}
namespace Auth.API.Endpoints;

public class OpenIdConfiguration : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/.well-known/openid-configuration", (HttpRequest req) =>
            {
                var issuer = $"{req.Scheme}://{req.Host}";
                return Results.Json(new
                {
                    issuer,
                    jwks_uri = $"{issuer}/.well-known/jwks.json"
                });
            })
            .AllowAnonymous()
            .WithName("OpenIdConfiguration");
    }
}
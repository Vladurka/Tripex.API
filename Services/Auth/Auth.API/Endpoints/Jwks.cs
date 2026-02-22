using System.Security.Cryptography;

namespace Auth.API.Endpoints;

public class Jwks : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/.well-known/jwks.json", ([FromServices] IConfiguration config) =>
            {
                var publicKeyPath = config["JwtOptions:PublicKeyPath"] ?? "keys/jwt-public.pem";
                var kid = config["JwtOptions:KeyId"] ?? "k1";

                var rsa = RSA.Create();
                rsa.ImportFromPem(File.ReadAllText(publicKeyPath));

                var rsaKey = new RsaSecurityKey(rsa) { KeyId = kid };

                var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaKey);
                jwk.Use = "sig";
                jwk.Alg = SecurityAlgorithms.RsaSha256;
                jwk.Kid = kid;

                return Results.Json(new { keys = new[] { jwk } });
            })
            .AllowAnonymous()
            .WithName("JWKS");
    }
}
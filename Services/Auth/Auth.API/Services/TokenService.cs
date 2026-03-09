namespace Auth.API.Services;
using System.Security.Cryptography;


public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICookiesService _cookiesManager;

    public TokenService(IOptions<JwtOptions> options, IHttpContextAccessor httpContextAccessor, ICookiesService cookiesManager)
    {
        _jwtOptions = options.Value;
        _httpContextAccessor = httpContextAccessor;
        _cookiesManager = cookiesManager;
    }

    public Guid GetUserIdByToken()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            throw new Exception("User is not authenticated");

        var idString = user.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

        if (idString == null)
            throw new NotFoundException("Id not found in token");

        return Guid.Parse(idString);
    }

    public void SetTokenWithId(Guid id, string name, int expiresMinutes)
    {
        string token = GenerateToken(id);
        if (expiresMinutes <= 0)
            throw new ArgumentNullException(nameof(expiresMinutes));
        if (!_cookiesManager.CookieExists(name))
            _cookiesManager.AddCookie(name, token, expiresMinutes); 
        else
            _cookiesManager.UpdateCookie(name, token, expiresMinutes); 
    }

    private string GenerateToken(Guid id)
    {
        Claim[] claims = [new("userId", id.ToString())];

        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(_jwtOptions.PrivateKeyPath));

        var signingKey = new RsaSecurityKey(rsa) { KeyId = _jwtOptions.KeyId };
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public TokenModel GenerateTokens(Guid userId)
    {
        var accessToken = GenerateToken(userId);
        var refreshToken = GenerateRefreshToken();

        return new TokenModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}


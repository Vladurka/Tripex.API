namespace Auth.API.Entities
{
    public class JwtOptions
    {
        public string TokenName { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string PrivateKeyPath { get; set; } = string.Empty;
        public string KeyId { get; set; } = string.Empty;
        public string PublicKeyPath { get; set; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
    }
}
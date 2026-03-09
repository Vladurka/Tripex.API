namespace Auth.API.Services
{
    public class CookiesService(IHttpContextAccessor httpContextAccessor) : ICookiesService
    {
        public void AddCookie(string name, string value, int minutes)
        {
            ValidateCookieParams(name, value, minutes);   

            if (CookieExists(name))
                throw new InvalidOperationException("Cookie with this name already exists");
            
            httpContextAccessor?.HttpContext?.Response.Cookies.Append(name, value, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(minutes)
            });
        }

        public string GetFromCookie(string name)
        {
            if (httpContextAccessor?.HttpContext?.Request.Cookies.TryGetValue(name, out string? value) == true)
                return value ?? string.Empty;
            return string.Empty;
        }

        public void DeleteCookie(string name)
        {
            if (!CookieExists(name))
                return;

            httpContextAccessor?.HttpContext?.Response.Cookies.Delete(name);
        }

        public void UpdateCookie(string name, string newValue, int minutes)
        {
            ValidateCookieParams(name, newValue, minutes);

            if (!CookieExists(name))
                throw new InvalidOperationException("Cookie is not found");

            httpContextAccessor?.HttpContext?.Response.Cookies.Append(name, newValue, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(minutes)
            });
        }

        private void ValidateCookieParams(string name, string value, int minutes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Cookie name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "Cookie value cannot be null or empty.");

            if (minutes <= 0)
                throw new ArgumentOutOfRangeException(nameof(minutes), "Expiration time must be greater than zero.");
        }

        public bool CookieExists(string name) =>
            httpContextAccessor?.HttpContext?.Request.Cookies.ContainsKey(name) == true;
    }
}


using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Auth;

public class HeaderBasedUserContext(IHttpContextAccessor httpContextAccessor) : IJwtHelper
{
    private const string UserIdHeader = "X-User-Id";

    public Guid GetUserIdByToken()
    {
        var context = httpContextAccessor.HttpContext
            ?? throw new UnauthorizedAccessException("No HTTP context available");

        if (!context.Request.Headers.TryGetValue(UserIdHeader, out var headerValue)
            || string.IsNullOrWhiteSpace(headerValue))
            throw new UnauthorizedAccessException("User is not authenticated");

        if (!Guid.TryParse(headerValue, out var userId))
            throw new NotFoundException("Invalid userId format in header");

        return userId;
    }
}

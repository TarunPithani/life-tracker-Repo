using System.Security.Claims;
using LifeTracker.Application.DTOs.Auth;

namespace LifeTracker.Application.Interfaces.Services;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokensAsync(
        long userId,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);
}

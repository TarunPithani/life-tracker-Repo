using System.Security.Claims;
using LifeTracker.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace LifeTracker.Infrastructure.Identity;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public long? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User?.FindFirstValue("sub");

            return long.TryParse(value, out var userId) ? userId : null;
        }
    }

    public string? Email =>
        User?.FindFirstValue(ClaimTypes.Email)
        ?? User?.FindFirstValue("email");

    public IReadOnlyCollection<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(claim => claim.Value).Distinct().ToArray()
        ?? [];

    public IReadOnlyCollection<string> Permissions =>
        User?.FindAll(Domain.Common.Constants.Permissions.ClaimType)
            .Select(claim => claim.Value)
            .Distinct()
            .ToArray()
        ?? [];

    public bool IsInRole(string role) =>
        Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

    public bool HasPermission(string permission) =>
        Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
}

namespace LifeTracker.Application.Interfaces.Services;

public interface ICurrentUserService
{
    long? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IReadOnlyCollection<string> Roles { get; }
    IReadOnlyCollection<string> Permissions { get; }
    bool IsInRole(string role);
    bool HasPermission(string permission);
}

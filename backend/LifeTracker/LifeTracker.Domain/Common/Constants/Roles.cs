namespace LifeTracker.Domain.Common.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string User = "User";

    public static IReadOnlyCollection<string> All { get; } =
    [
        Admin,
        Manager,
        User
    ];
}

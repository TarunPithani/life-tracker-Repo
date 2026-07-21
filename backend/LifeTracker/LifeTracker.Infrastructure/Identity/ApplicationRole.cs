using Microsoft.AspNetCore.Identity;

namespace LifeTracker.Infrastructure.Identity;

public class ApplicationRole : IdentityRole<long>
{
    public string? Description { get; set; }

    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName)
        : base(roleName)
    {
    }
}

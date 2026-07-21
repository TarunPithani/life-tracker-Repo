using Microsoft.AspNetCore.Identity;

namespace LifeTracker.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<long>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public bool IsActive { get; set; } = true;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}

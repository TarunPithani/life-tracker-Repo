using LifeTracker.Domain.Common;

namespace LifeTracker.Infrastructure.Identity;

public class RefreshToken : BaseEntity
{
    public long UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresOn { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedByIp { get; set; }
    public DateTimeOffset? RevokedOn { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresOn;
    public bool IsRevoked => RevokedOn.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    public ApplicationUser User { get; set; } = null!;
}

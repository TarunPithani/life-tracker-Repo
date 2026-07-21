using System.ComponentModel.DataAnnotations;

namespace LifeTracker.Domain.Common;

public class AuditEntity : BaseEntity, IAuditEntity
{
    public DateTimeOffset? CreatedOn { get; set; }
    public long? CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public long? ModifiedBy { get; set; }

    [Timestamp]
    public byte[]? Version { get; set; }
}
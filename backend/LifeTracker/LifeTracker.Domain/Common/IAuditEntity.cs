namespace LifeTracker.Domain.Common;

public interface IAuditEntity : IEntity
{
    DateTimeOffset? CreatedOn { get; set; }
    long? CreatedBy { get; set; }
    DateTimeOffset? ModifiedOn { get; set; }
    long? ModifiedBy { get; set; }
    byte[]? Version { get; set; }
}
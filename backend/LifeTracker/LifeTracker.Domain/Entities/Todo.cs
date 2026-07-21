using LifeTracker.Domain.Common;

namespace LifeTracker.Domain.Entities;

public class Todo : AuditEntity
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateOnly DueDate { get; set; }
    public int Priority { get; set; }
}

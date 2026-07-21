namespace LifeTracker.Application.DTOs.Todos;

public class TodoResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateOnly DueDate { get; set; }
    public int Priority { get; set; }
    public DateTimeOffset? CreatedOn { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public byte[]? Version { get; set; }
}

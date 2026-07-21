namespace LifeTracker.Application.DTOs.Todos;

public class TodoSummaryResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateOnly DueDate { get; set; }
    public int Priority { get; set; }
}

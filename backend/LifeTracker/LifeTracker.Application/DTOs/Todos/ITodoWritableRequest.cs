namespace LifeTracker.Application.DTOs.Todos;

public interface ITodoWritableRequest
{
    string Title { get; set; }
    string? Description { get; set; }
    DateOnly DueDate { get; set; }
    int Priority { get; set; }
}

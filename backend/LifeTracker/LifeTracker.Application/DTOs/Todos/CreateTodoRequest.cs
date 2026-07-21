using System.ComponentModel.DataAnnotations;
using LifeTracker.Domain.Common.Constants;

namespace LifeTracker.Application.DTOs.Todos;

public class CreateTodoRequest : ITodoWritableRequest
{
    [Required]
    [MaxLength(TodoConstants.TitleMaxLength)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(TodoConstants.DescriptionMaxLength)]
    public string? Description { get; set; }

    [Required]
    public DateOnly DueDate { get; set; }

    public int Priority { get; set; }
}

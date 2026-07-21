using FluentValidation;
using LifeTracker.Application.DTOs.Todos;
using LifeTracker.Domain.Common.Constants;

namespace LifeTracker.Application.Validators.Todos;

public abstract class TodoRequestValidatorBase<TRequest> : AbstractValidator<TRequest>
    where TRequest : ITodoWritableRequest
{
    protected TodoRequestValidatorBase()
    {
        RuleFor(request => request.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(TodoConstants.TitleMaxLength)
            .WithMessage($"Title must not exceed {TodoConstants.TitleMaxLength} characters.");

        RuleFor(request => request.Description)
            .MaximumLength(TodoConstants.DescriptionMaxLength)
            .When(request => request.Description is not null)
            .WithMessage($"Description must not exceed {TodoConstants.DescriptionMaxLength} characters.");

        RuleFor(request => request.DueDate)
            .GreaterThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Due date must be today or a future date.");
    }
}

using FluentValidation;
using LifeTracker.Application.DTOs.Todos;
using LifeTracker.Application.Exceptions;
using LifeTracker.Application.Interfaces.Repositories;
using LifeTracker.Application.Interfaces.Services;
using LifeTracker.Domain.Entities;

namespace LifeTracker.Application.Services;

public class TodoService(
    ITodoRepository todoRepository,
    IValidator<CreateTodoRequest> createTodoValidator,
    IValidator<UpdateTodoRequest> updateTodoValidator) : ITodoService
{
    public async Task<TodoResponse> CreateTodoAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken = default)
    {
        await createTodoValidator.ValidateAndThrowAsync(request, cancellationToken);

        var todo = new Todo
        {
            Title = request.Title.Trim(),
            Description = NormalizeDescription(request.Description),
            DueDate = request.DueDate,
            Priority = request.Priority,
            IsCompleted = false,
            CreatedOn = DateTimeOffset.UtcNow
        };

        await todoRepository.AddAsync(todo, cancellationToken);

        return MapToResponse(todo);
    }

    public async Task<TodoResponse> UpdateTodoAsync(
        long id,
        UpdateTodoRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateTodoValidator.ValidateAndThrowAsync(request, cancellationToken);

        var todo = await GetRequiredTodoAsync(id, cancellationToken);

        todo.Title = request.Title.Trim();
        todo.Description = NormalizeDescription(request.Description);
        todo.DueDate = request.DueDate;
        todo.Priority = request.Priority;
        todo.ModifiedOn = DateTimeOffset.UtcNow;

        if (request.Version is not null)
        {
            todo.Version = request.Version;
        }

        await todoRepository.UpdateAsync(todo, cancellationToken);

        return MapToResponse(todo);
    }

    public async Task DeleteTodoAsync(long id, CancellationToken cancellationToken = default)
    {
        var todo = await GetRequiredTodoAsync(id, cancellationToken);
        await todoRepository.DeleteAsync(todo, cancellationToken);
    }

    public async Task<TodoResponse> CompleteTodoAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var todo = await GetRequiredTodoAsync(id, cancellationToken);

        if (!todo.IsCompleted)
        {
            todo.IsCompleted = true;
            todo.ModifiedOn = DateTimeOffset.UtcNow;
            await todoRepository.UpdateAsync(todo, cancellationToken);
        }

        return MapToResponse(todo);
    }

    public async Task<TodoResponse> GetTodoAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var todo = await GetRequiredTodoAsync(id, cancellationToken);
        return MapToResponse(todo);
    }

    public async Task<IReadOnlyCollection<TodoSummaryResponse>> GetTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var todos = await todoRepository.GetListAsync(cancellationToken: cancellationToken);
        return todos.Select(MapToSummary).ToList();
    }

    public async Task<IReadOnlyCollection<TodoSummaryResponse>> GetCompletedTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var todos = await todoRepository.GetListAsync(
            todo => todo.IsCompleted,
            cancellationToken);

        return todos.Select(MapToSummary).ToList();
    }

    public async Task<IReadOnlyCollection<TodoSummaryResponse>> GetPendingTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var todos = await todoRepository.GetListAsync(
            todo => !todo.IsCompleted,
            cancellationToken);

        return todos.Select(MapToSummary).ToList();
    }

    private async Task<Todo> GetRequiredTodoAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetByIdAsync(id, cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException(nameof(Todo), id);
        }

        return todo;
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private static TodoResponse MapToResponse(Todo todo)
    {
        return new TodoResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            DueDate = todo.DueDate,
            Priority = todo.Priority,
            CreatedOn = todo.CreatedOn,
            ModifiedOn = todo.ModifiedOn,
            Version = todo.Version
        };
    }

    private static TodoSummaryResponse MapToSummary(Todo todo)
    {
        return new TodoSummaryResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            IsCompleted = todo.IsCompleted,
            DueDate = todo.DueDate,
            Priority = todo.Priority
        };
    }
}

using LifeTracker.Application.DTOs.Todos;

namespace LifeTracker.Application.Interfaces.Services;

public interface ITodoService
{
    Task<TodoResponse> CreateTodoAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken = default);

    Task<TodoResponse> UpdateTodoAsync(
        long id,
        UpdateTodoRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteTodoAsync(long id, CancellationToken cancellationToken = default);

    Task<TodoResponse> CompleteTodoAsync(long id, CancellationToken cancellationToken = default);

    Task<TodoResponse> GetTodoAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TodoSummaryResponse>> GetTodosAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TodoSummaryResponse>> GetCompletedTodosAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TodoSummaryResponse>> GetPendingTodosAsync(
        CancellationToken cancellationToken = default);
}

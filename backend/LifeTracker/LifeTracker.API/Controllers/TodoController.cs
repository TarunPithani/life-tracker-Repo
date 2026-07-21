using LifeTracker.Application.DTOs.Todos;
using LifeTracker.Application.Interfaces.Services;
using LifeTracker.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeTracker.API.Controllers;

[ApiController]
[Authorize]
[Route("api/todos")]
public class TodoController(ITodoService todoService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = Permissions.Todo.Create)]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoResponse>> Create(
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.CreateTodoAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    [HttpGet]
    [Authorize(Policy = Permissions.Todo.Read)]
    [ProducesResponseType(typeof(IReadOnlyCollection<TodoSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TodoSummaryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var todos = await todoService.GetTodosAsync(cancellationToken);
        return Ok(todos);
    }

    [HttpGet("completed")]
    [Authorize(Policy = Permissions.Todo.Read)]
    [ProducesResponseType(typeof(IReadOnlyCollection<TodoSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TodoSummaryResponse>>> GetCompleted(
        CancellationToken cancellationToken)
    {
        var todos = await todoService.GetCompletedTodosAsync(cancellationToken);
        return Ok(todos);
    }

    [HttpGet("pending")]
    [Authorize(Policy = Permissions.Todo.Read)]
    [ProducesResponseType(typeof(IReadOnlyCollection<TodoSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TodoSummaryResponse>>> GetPending(
        CancellationToken cancellationToken)
    {
        var todos = await todoService.GetPendingTodosAsync(cancellationToken);
        return Ok(todos);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = Permissions.Todo.Read)]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoResponse>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.GetTodoAsync(id, cancellationToken);
        return Ok(todo);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = Permissions.Todo.Update)]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoResponse>> Update(
        long id,
        [FromBody] UpdateTodoRequest request,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.UpdateTodoAsync(id, request, cancellationToken);
        return Ok(todo);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = Permissions.Todo.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await todoService.DeleteTodoAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:long}/complete")]
    [Authorize(Policy = Permissions.Todo.Update)]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoResponse>> Complete(
        long id,
        CancellationToken cancellationToken)
    {
        var todo = await todoService.CompleteTodoAsync(id, cancellationToken);
        return Ok(todo);
    }
}

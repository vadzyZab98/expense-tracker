using System.Security.Claims;
using ExpenseTracker.Logic.DTOs;
using ExpenseTracker.Logic.Expenses.CreateExpense;
using ExpenseTracker.Logic.Expenses.DeleteExpense;
using ExpenseTracker.Logic.Expenses.GetExpenseById;
using ExpenseTracker.Logic.Expenses.GetExpenses;
using ExpenseTracker.Logic.Expenses.UpdateExpense;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Route("api/expenses")]
[Authorize]
public class ExpensesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ExpensesController(IMediator mediator) => _mediator = mediator;

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub")!);

    public record ExpenseRequest(decimal Amount, string Description, DateTime Date, int CategoryId);

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExpenseResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetExpensesQuery(CurrentUserId), ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetExpenseByIdQuery(id, CurrentUserId), ct);
        return result.IsSuccess ? Ok(result.Value) : MapError(result.Error!);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(ExpenseRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateExpenseCommand(CurrentUserId, request.Amount, request.Description, request.Date, request.CategoryId), ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value!.Id }, result.Value)
            : MapError(result.Error!);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, ExpenseRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpdateExpenseCommand(id, CurrentUserId, request.Amount, request.Description, request.Date, request.CategoryId), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteExpenseCommand(id, CurrentUserId), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }
}

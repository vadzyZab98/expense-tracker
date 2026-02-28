using ExpenseTracker.Logic.Budgets.CreateBudget;
using ExpenseTracker.Logic.Budgets.DeleteBudget;
using ExpenseTracker.Logic.Budgets.GetBudgets;
using ExpenseTracker.Logic.Budgets.GetBudgetsByMonth;
using ExpenseTracker.Logic.Budgets.UpdateBudget;
using ExpenseTracker.Logic.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Route("api/budgets")]
[Authorize]
public class BudgetsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public BudgetsController(IMediator mediator) => _mediator = mediator;

    public record BudgetRequest(int CategoryId, int Year, int Month, decimal Amount);

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MonthlyBudgetResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
    {
        if (year.HasValue && month.HasValue)
        {
            var byMonth = await _mediator.Send(
                new GetBudgetsByMonthQuery(CurrentUserId, year.Value, month.Value), ct);
            return Ok(byMonth);
        }

        var result = await _mediator.Send(new GetBudgetsQuery(CurrentUserId), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MonthlyBudgetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(BudgetRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateBudgetCommand(
                CurrentUserId, request.CategoryId, request.Year, request.Month, request.Amount), ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value!.Id }, result.Value)
            : MapError(result.Error!);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, BudgetRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpdateBudgetCommand(
                id, CurrentUserId, request.CategoryId, request.Year, request.Month, request.Amount), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteBudgetCommand(id, CurrentUserId), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }
}

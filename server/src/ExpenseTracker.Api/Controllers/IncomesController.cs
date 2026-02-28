using ExpenseTracker.Logic.DTOs;
using ExpenseTracker.Logic.Incomes.CreateIncome;
using ExpenseTracker.Logic.Incomes.DeleteIncome;
using ExpenseTracker.Logic.Incomes.GetIncomeById;
using ExpenseTracker.Logic.Incomes.GetIncomes;
using ExpenseTracker.Logic.Incomes.UpdateIncome;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Route("api/incomes")]
[Authorize]
public class IncomesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public IncomesController(IMediator mediator) => _mediator = mediator;

    public record IncomeRequest(decimal Amount, DateTime Date, int IncomeCategoryId);

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<IncomeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetIncomesQuery(CurrentUserId), ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IncomeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetIncomeByIdQuery(id, CurrentUserId), ct);
        return result.IsSuccess ? Ok(result.Value) : MapError(result.Error!);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IncomeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(IncomeRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateIncomeCommand(CurrentUserId, request.Amount, request.Date, request.IncomeCategoryId), ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value!.Id }, result.Value)
            : MapError(result.Error!);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, IncomeRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpdateIncomeCommand(id, CurrentUserId, request.Amount, request.Date, request.IncomeCategoryId), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteIncomeCommand(id, CurrentUserId), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }
}

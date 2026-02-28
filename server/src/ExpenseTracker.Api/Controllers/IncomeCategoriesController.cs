using ExpenseTracker.Api.Auth;
using ExpenseTracker.Logic.DTOs;
using ExpenseTracker.Logic.IncomeCategories.CreateIncomeCategory;
using ExpenseTracker.Logic.IncomeCategories.DeleteIncomeCategory;
using ExpenseTracker.Logic.IncomeCategories.GetIncomeCategories;
using ExpenseTracker.Logic.IncomeCategories.GetIncomeCategoryById;
using ExpenseTracker.Logic.IncomeCategories.UpdateIncomeCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Route("api/income-categories")]
[Authorize(Policy = ApiPolicies.CanManageCategories)]
public class IncomeCategoriesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public IncomeCategoriesController(IMediator mediator) => _mediator = mediator;

    public record IncomeCategoryRequest(string Name, string Color);

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<IncomeCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetIncomeCategoriesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IncomeCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetIncomeCategoryByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : MapError(result.Error!);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IncomeCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(IncomeCategoryRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateIncomeCategoryCommand(request.Name, request.Color), ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value!.Id }, result.Value)
            : MapError(result.Error!);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, IncomeCategoryRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpdateIncomeCategoryCommand(id, request.Name, request.Color), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteIncomeCategoryCommand(id), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }
}

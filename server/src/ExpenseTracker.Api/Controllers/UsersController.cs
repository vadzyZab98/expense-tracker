using ExpenseTracker.Api.Auth;
using ExpenseTracker.Logic.DTOs;
using ExpenseTracker.Logic.Users.AssignRole;
using ExpenseTracker.Logic.Users.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Route("api/users")]
[Authorize(Policy = ApiPolicies.CanManageUsers)]
public class UsersController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    public record RoleRequest(string Role);

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUsersQuery(), ct);
        return Ok(result);
    }

    [HttpPut("{id}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AssignRole(int id, RoleRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new AssignRoleCommand(id, request.Role), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }
}

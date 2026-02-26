using ExpenseTracker.Logic.Common;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult MapError(DomainError error)
    {
        var problem = new ProblemDetails
        {
            Status = error.Code switch
            {
                ErrorCode.NotFound => StatusCodes.Status404NotFound,
                ErrorCode.Conflict => StatusCodes.Status409Conflict,
                ErrorCode.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            },
            Title = error.Code switch
            {
                ErrorCode.NotFound => "Not Found",
                ErrorCode.Conflict => "Conflict",
                ErrorCode.Unauthorized => "Unauthorized",
                _ => "Internal Server Error"
            },
            Detail = error.Message
        };

        return StatusCode(problem.Status!.Value, problem);
    }
}

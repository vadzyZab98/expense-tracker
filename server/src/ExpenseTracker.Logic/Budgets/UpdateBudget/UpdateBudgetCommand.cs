using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Budgets.UpdateBudget;

public sealed record UpdateBudgetCommand(
    int Id,
    int UserId,
    int CategoryId,
    int Year,
    int Month,
    decimal Amount) : IRequest<Result>;

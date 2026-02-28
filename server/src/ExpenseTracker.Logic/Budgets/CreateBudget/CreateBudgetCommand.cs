using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Budgets.CreateBudget;

public sealed record CreateBudgetCommand(
    int UserId,
    int CategoryId,
    int Year,
    int Month,
    decimal Amount) : IRequest<Result<MonthlyBudgetResponse>>;

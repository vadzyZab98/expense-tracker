using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Budgets.GetBudgetsByMonth;

public sealed record GetBudgetsByMonthQuery(int UserId, int Year, int Month)
    : IRequest<IReadOnlyList<MonthlyBudgetResponse>>;

using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Budgets.GetBudgets;

public sealed record GetBudgetsQuery(int UserId) : IRequest<IReadOnlyList<MonthlyBudgetResponse>>;

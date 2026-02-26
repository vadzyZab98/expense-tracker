using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.GetExpenses;

public sealed record GetExpensesQuery(int UserId) : IRequest<IReadOnlyList<ExpenseResponse>>;

using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Budgets.DeleteBudget;

public sealed record DeleteBudgetCommand(int Id, int UserId) : IRequest<Result>;

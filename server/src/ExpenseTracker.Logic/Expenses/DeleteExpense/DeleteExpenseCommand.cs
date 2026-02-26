using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.DeleteExpense;

public sealed record DeleteExpenseCommand(int Id, int UserId) : IRequest<Result>;

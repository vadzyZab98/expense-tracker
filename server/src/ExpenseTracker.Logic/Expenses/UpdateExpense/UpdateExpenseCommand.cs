using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.UpdateExpense;

public sealed record UpdateExpenseCommand(
    int Id,
    int UserId,
    decimal Amount,
    string Description,
    DateTime Date,
    int CategoryId) : IRequest<Result>;

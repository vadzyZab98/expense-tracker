using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.CreateExpense;

public sealed record CreateExpenseCommand(
    int UserId,
    decimal Amount,
    string Description,
    DateTime Date,
    int CategoryId) : IRequest<Result<ExpenseResponse>>;

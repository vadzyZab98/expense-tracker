using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Incomes.UpdateIncome;

public sealed record UpdateIncomeCommand(
    int Id,
    int UserId,
    decimal Amount,
    DateTime Date,
    int IncomeCategoryId) : IRequest<Result>;

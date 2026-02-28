using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Incomes.CreateIncome;

public sealed record CreateIncomeCommand(
    int UserId,
    decimal Amount,
    DateTime Date,
    int IncomeCategoryId) : IRequest<Result<IncomeResponse>>;

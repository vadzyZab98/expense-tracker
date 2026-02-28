using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Incomes.DeleteIncome;

public sealed record DeleteIncomeCommand(int Id, int UserId) : IRequest<Result>;

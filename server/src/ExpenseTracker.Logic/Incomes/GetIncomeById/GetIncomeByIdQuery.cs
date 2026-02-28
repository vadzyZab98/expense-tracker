using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Incomes.GetIncomeById;

public sealed record GetIncomeByIdQuery(int Id, int UserId) : IRequest<Result<IncomeResponse>>;

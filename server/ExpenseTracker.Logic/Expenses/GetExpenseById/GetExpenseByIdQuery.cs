using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.GetExpenseById;

public sealed record GetExpenseByIdQuery(int Id, int UserId) : IRequest<Result<ExpenseResponse>>;

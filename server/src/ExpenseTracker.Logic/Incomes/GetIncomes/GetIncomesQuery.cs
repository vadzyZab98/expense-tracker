using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Incomes.GetIncomes;

public sealed record GetIncomesQuery(int UserId) : IRequest<IReadOnlyList<IncomeResponse>>;

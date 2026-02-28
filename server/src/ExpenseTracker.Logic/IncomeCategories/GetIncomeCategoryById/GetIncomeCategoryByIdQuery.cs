using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.IncomeCategories.GetIncomeCategoryById;

public sealed record GetIncomeCategoryByIdQuery(int Id) : IRequest<Result<IncomeCategoryResponse>>;

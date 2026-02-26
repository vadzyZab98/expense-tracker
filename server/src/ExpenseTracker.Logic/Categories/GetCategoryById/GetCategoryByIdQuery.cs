using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Categories.GetCategoryById;

public sealed record GetCategoryByIdQuery(int Id) : IRequest<Result<CategoryResponse>>;

using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Categories.CreateCategory;

public sealed record CreateCategoryCommand(string Name, string Color) : IRequest<Result<CategoryResponse>>;

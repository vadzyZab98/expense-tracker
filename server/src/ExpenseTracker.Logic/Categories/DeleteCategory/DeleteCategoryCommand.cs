using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(int Id) : IRequest<Result>;

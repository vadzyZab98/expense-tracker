using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(int Id, string Name, string Color) : IRequest<Result>;

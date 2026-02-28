using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.IncomeCategories.UpdateIncomeCategory;

public sealed record UpdateIncomeCategoryCommand(int Id, string Name, string Color)
    : IRequest<Result>;

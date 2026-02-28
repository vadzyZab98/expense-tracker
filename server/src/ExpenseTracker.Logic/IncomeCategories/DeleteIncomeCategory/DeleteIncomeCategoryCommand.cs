using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.IncomeCategories.DeleteIncomeCategory;

public sealed record DeleteIncomeCategoryCommand(int Id) : IRequest<Result>;

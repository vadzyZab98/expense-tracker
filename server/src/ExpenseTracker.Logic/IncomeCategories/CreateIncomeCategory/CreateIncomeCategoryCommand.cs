using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.IncomeCategories.CreateIncomeCategory;

public sealed record CreateIncomeCategoryCommand(string Name, string Color)
    : IRequest<Result<IncomeCategoryResponse>>;

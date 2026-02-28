using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.IncomeCategories.GetIncomeCategories;

public sealed record GetIncomeCategoriesQuery : IRequest<IReadOnlyList<IncomeCategoryResponse>>;

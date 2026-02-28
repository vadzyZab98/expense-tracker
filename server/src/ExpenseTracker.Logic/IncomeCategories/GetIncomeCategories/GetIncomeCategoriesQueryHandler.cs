using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.IncomeCategories.GetIncomeCategories;

public sealed class GetIncomeCategoriesQueryHandler
    : IRequestHandler<GetIncomeCategoriesQuery, IReadOnlyList<IncomeCategoryResponse>>
{
    private readonly IIncomeCategoryRepository _incomeCategories;

    public GetIncomeCategoriesQueryHandler(IIncomeCategoryRepository incomeCategories)
    {
        _incomeCategories = incomeCategories;
    }

    public async Task<IReadOnlyList<IncomeCategoryResponse>> Handle(
        GetIncomeCategoriesQuery request, CancellationToken ct)
    {
        var categories = await _incomeCategories.GetAllAsync(ct);

        return categories
            .Select(c => new IncomeCategoryResponse(c.Id, c.Name, c.Color))
            .ToList();
    }
}

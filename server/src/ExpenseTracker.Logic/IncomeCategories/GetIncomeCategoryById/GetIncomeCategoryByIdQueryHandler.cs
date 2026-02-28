using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.IncomeCategories.GetIncomeCategoryById;

public sealed class GetIncomeCategoryByIdQueryHandler
    : IRequestHandler<GetIncomeCategoryByIdQuery, Result<IncomeCategoryResponse>>
{
    private readonly IIncomeCategoryRepository _incomeCategories;

    public GetIncomeCategoryByIdQueryHandler(IIncomeCategoryRepository incomeCategories)
    {
        _incomeCategories = incomeCategories;
    }

    public async Task<Result<IncomeCategoryResponse>> Handle(
        GetIncomeCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await _incomeCategories.FindByIdAsync(request.Id, ct);
        if (category is null)
            return Result<IncomeCategoryResponse>.Failure(
                DomainError.NotFound("IncomeCategory", request.Id));

        return Result<IncomeCategoryResponse>.Success(
            new IncomeCategoryResponse(category.Id, category.Name, category.Color));
    }
}

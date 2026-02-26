using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Categories.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponse>>
{
    private readonly ICategoryRepository _categories;

    public GetCategoryByIdQueryHandler(ICategoryRepository categories)
    {
        _categories = categories;
    }

    public async Task<Result<CategoryResponse>> Handle(
        GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await _categories.FindByIdAsync(request.Id, ct);

        if (category is null)
            return Result<CategoryResponse>.Failure(
                DomainError.NotFound("Category", request.Id));

        return Result<CategoryResponse>.Success(
            new CategoryResponse(category.Id, category.Name, category.Color));
    }
}

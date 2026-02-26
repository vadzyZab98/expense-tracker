using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Categories.UpdateCategory;

public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(ICategoryRepository categories, IUnitOfWork unitOfWork)
    {
        _categories = categories;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await _categories.FindByIdAsync(request.Id, ct);
        if (category is null)
            return Result.Failure(DomainError.NotFound("Category", request.Id));

        category.Name = request.Name;
        category.Color = request.Color;

        _categories.Update(category);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

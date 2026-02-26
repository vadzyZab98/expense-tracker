using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Categories.DeleteCategory;

public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(ICategoryRepository categories, IUnitOfWork unitOfWork)
    {
        _categories = categories;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await _categories.FindByIdAsync(request.Id, ct);
        if (category is null)
            return Result.Failure(DomainError.NotFound("Category", request.Id));

        if (await _categories.HasExpensesAsync(request.Id, ct))
            return Result.Failure(DomainError.Conflict(
                $"Category with id '{request.Id}' cannot be deleted because it has linked expenses."));

        _categories.Delete(category);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

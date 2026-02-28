using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.IncomeCategories.UpdateIncomeCategory;

public sealed class UpdateIncomeCategoryCommandHandler
    : IRequestHandler<UpdateIncomeCategoryCommand, Result>
{
    private readonly IIncomeCategoryRepository _incomeCategories;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIncomeCategoryCommandHandler(
        IIncomeCategoryRepository incomeCategories,
        IUnitOfWork unitOfWork)
    {
        _incomeCategories = incomeCategories;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateIncomeCategoryCommand request, CancellationToken ct)
    {
        var category = await _incomeCategories.FindByIdAsync(request.Id, ct);
        if (category is null)
            return Result.Failure(DomainError.NotFound("IncomeCategory", request.Id));

        category.Name = request.Name;
        category.Color = request.Color;

        _incomeCategories.Update(category);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.IncomeCategories.DeleteIncomeCategory;

public sealed class DeleteIncomeCategoryCommandHandler
    : IRequestHandler<DeleteIncomeCategoryCommand, Result>
{
    private readonly IIncomeCategoryRepository _incomeCategories;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIncomeCategoryCommandHandler(
        IIncomeCategoryRepository incomeCategories,
        IUnitOfWork unitOfWork)
    {
        _incomeCategories = incomeCategories;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteIncomeCategoryCommand request, CancellationToken ct)
    {
        var category = await _incomeCategories.FindByIdAsync(request.Id, ct);
        if (category is null)
            return Result.Failure(DomainError.NotFound("IncomeCategory", request.Id));

        var hasIncomes = await _incomeCategories.HasIncomesAsync(request.Id, ct);
        if (hasIncomes)
            return Result.Failure(DomainError.Conflict(
                "Cannot delete income category because it has associated income records."));

        _incomeCategories.Delete(category);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

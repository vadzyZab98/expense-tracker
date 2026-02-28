using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.IncomeCategories.CreateIncomeCategory;

public sealed class CreateIncomeCategoryCommandHandler
    : IRequestHandler<CreateIncomeCategoryCommand, Result<IncomeCategoryResponse>>
{
    private readonly IIncomeCategoryRepository _incomeCategories;
    private readonly IUnitOfWork _unitOfWork;

    public CreateIncomeCategoryCommandHandler(
        IIncomeCategoryRepository incomeCategories,
        IUnitOfWork unitOfWork)
    {
        _incomeCategories = incomeCategories;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IncomeCategoryResponse>> Handle(
        CreateIncomeCategoryCommand request, CancellationToken ct)
    {
        var category = new IncomeCategory
        {
            Name = request.Name,
            Color = request.Color
        };

        await _incomeCategories.AddAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<IncomeCategoryResponse>.Success(
            new IncomeCategoryResponse(category.Id, category.Name, category.Color));
    }
}

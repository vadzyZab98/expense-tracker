using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Incomes.CreateIncome;

public sealed class CreateIncomeCommandHandler
    : IRequestHandler<CreateIncomeCommand, Result<IncomeResponse>>
{
    private readonly IIncomeRepository _incomes;
    private readonly IIncomeCategoryRepository _incomeCategories;
    private readonly IUnitOfWork _unitOfWork;

    public CreateIncomeCommandHandler(
        IIncomeRepository incomes,
        IIncomeCategoryRepository incomeCategories,
        IUnitOfWork unitOfWork)
    {
        _incomes = incomes;
        _incomeCategories = incomeCategories;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IncomeResponse>> Handle(
        CreateIncomeCommand request, CancellationToken ct)
    {
        var category = await _incomeCategories.FindByIdAsync(request.IncomeCategoryId, ct);
        if (category is null)
            return Result<IncomeResponse>.Failure(
                DomainError.NotFound("IncomeCategory", request.IncomeCategoryId));

        var income = new Income
        {
            UserId = request.UserId,
            IncomeCategoryId = request.IncomeCategoryId,
            Amount = request.Amount,
            Date = request.Date
        };

        await _incomes.AddAsync(income, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<IncomeResponse>.Success(new IncomeResponse(
            income.Id,
            income.Amount,
            income.Date,
            income.IncomeCategoryId,
            new IncomeCategoryResponse(category.Id, category.Name, category.Color)));
    }
}

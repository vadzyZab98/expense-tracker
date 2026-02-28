using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Budgets.CreateBudget;

public sealed class CreateBudgetCommandHandler
    : IRequestHandler<CreateBudgetCommand, Result<MonthlyBudgetResponse>>
{
    private readonly IMonthlyBudgetRepository _budgets;
    private readonly ICategoryRepository _categories;
    private readonly IIncomeRepository _incomes;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBudgetCommandHandler(
        IMonthlyBudgetRepository budgets,
        ICategoryRepository categories,
        IIncomeRepository incomes,
        IUnitOfWork unitOfWork)
    {
        _budgets = budgets;
        _categories = categories;
        _incomes = incomes;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MonthlyBudgetResponse>> Handle(
        CreateBudgetCommand request, CancellationToken ct)
    {
        var category = await _categories.FindByIdAsync(request.CategoryId, ct);
        if (category is null)
            return Result<MonthlyBudgetResponse>.Failure(
                DomainError.NotFound("Category", request.CategoryId));

        var totalIncome = await _incomes.GetTotalForMonthAsync(
            request.UserId, request.Year, request.Month, ct);
        if (totalIncome == 0)
            return Result<MonthlyBudgetResponse>.Failure(
                DomainError.Conflict(
                    $"No income recorded for {request.Year}-{request.Month:D2}. Budget creation is forbidden when total income is zero."));

        var currentTotalBudgets = await _budgets.GetTotalForMonthAsync(
            request.UserId, request.Year, request.Month, ct);
        var newTotalBudgets = currentTotalBudgets + request.Amount;
        if (newTotalBudgets > totalIncome)
            return Result<MonthlyBudgetResponse>.Failure(
                DomainError.Conflict(
                    $"Total budgets ({newTotalBudgets:F2}) would exceed total income ({totalIncome:F2}) for {request.Year}-{request.Month:D2}."));

        var budget = new MonthlyBudget
        {
            UserId = request.UserId,
            CategoryId = request.CategoryId,
            Year = request.Year,
            Month = request.Month,
            Amount = request.Amount
        };

        await _budgets.AddAsync(budget, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<MonthlyBudgetResponse>.Success(new MonthlyBudgetResponse(
            budget.Id,
            budget.CategoryId,
            new CategoryResponse(category.Id, category.Name, category.Color),
            budget.Year,
            budget.Month,
            budget.Amount));
    }
}

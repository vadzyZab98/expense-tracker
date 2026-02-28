using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Budgets.UpdateBudget;

public sealed class UpdateBudgetCommandHandler
    : IRequestHandler<UpdateBudgetCommand, Result>
{
    private readonly IMonthlyBudgetRepository _budgets;
    private readonly ICategoryRepository _categories;
    private readonly IIncomeRepository _incomes;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBudgetCommandHandler(
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

    public async Task<Result> Handle(UpdateBudgetCommand request, CancellationToken ct)
    {
        var budget = await _budgets.FindByIdAndUserAsync(request.Id, request.UserId, ct);
        if (budget is null)
            return Result.Failure(DomainError.NotFound("MonthlyBudget", request.Id));

        var category = await _categories.FindByIdAsync(request.CategoryId, ct);
        if (category is null)
            return Result.Failure(DomainError.NotFound("Category", request.CategoryId));

        var totalIncome = await _incomes.GetTotalForMonthAsync(
            request.UserId, request.Year, request.Month, ct);
        if (totalIncome == 0)
            return Result.Failure(DomainError.Conflict(
                $"No income recorded for {request.Year}-{request.Month:D2}. Budget update is forbidden when total income is zero."));

        // Calculate new total budgets for the target month
        var targetMonthChanged = budget.Year != request.Year || budget.Month != request.Month;
        var currentTotalBudgets = await _budgets.GetTotalForMonthAsync(
            request.UserId, request.Year, request.Month, ct);

        var newTotalBudgets = targetMonthChanged
            ? currentTotalBudgets + request.Amount
            : currentTotalBudgets - budget.Amount + request.Amount;

        if (newTotalBudgets > totalIncome)
            return Result.Failure(DomainError.Conflict(
                $"Total budgets ({newTotalBudgets:F2}) would exceed total income ({totalIncome:F2}) for {request.Year}-{request.Month:D2}."));

        budget.CategoryId = request.CategoryId;
        budget.Year = request.Year;
        budget.Month = request.Month;
        budget.Amount = request.Amount;

        _budgets.Update(budget);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

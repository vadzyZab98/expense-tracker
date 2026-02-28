using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Incomes.UpdateIncome;

public sealed class UpdateIncomeCommandHandler
    : IRequestHandler<UpdateIncomeCommand, Result>
{
    private readonly IIncomeRepository _incomes;
    private readonly IIncomeCategoryRepository _incomeCategories;
    private readonly IExpenseRepository _expenses;
    private readonly IMonthlyBudgetRepository _budgets;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIncomeCommandHandler(
        IIncomeRepository incomes,
        IIncomeCategoryRepository incomeCategories,
        IExpenseRepository expenses,
        IMonthlyBudgetRepository budgets,
        IUnitOfWork unitOfWork)
    {
        _incomes = incomes;
        _incomeCategories = incomeCategories;
        _expenses = expenses;
        _budgets = budgets;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateIncomeCommand request, CancellationToken ct)
    {
        var income = await _incomes.FindByIdAndUserAsync(request.Id, request.UserId, ct);
        if (income is null)
            return Result.Failure(DomainError.NotFound("Income", request.Id));

        var category = await _incomeCategories.FindByIdAsync(request.IncomeCategoryId, ct);
        if (category is null)
            return Result.Failure(DomainError.NotFound("IncomeCategory", request.IncomeCategoryId));

        var oldYear = income.Date.Year;
        var oldMonth = income.Date.Month;
        var newYear = request.Date.Year;
        var newMonth = request.Date.Month;

        // Check constraints on the old month if amount decreased or month changed
        var monthChanged = oldYear != newYear || oldMonth != newMonth;
        if (monthChanged || request.Amount < income.Amount)
        {
            // Validate old month: removing/reducing income from it
            var oldTotalIncome = await _incomes.GetTotalForMonthAsync(
                request.UserId, oldYear, oldMonth, ct);
            var newOldMonthIncome = monthChanged
                ? oldTotalIncome - income.Amount
                : oldTotalIncome - income.Amount + request.Amount;

            var oldMonthBudgets = await _budgets.GetTotalForMonthAsync(
                request.UserId, oldYear, oldMonth, ct);
            if (newOldMonthIncome < oldMonthBudgets)
                return Result.Failure(DomainError.Conflict(
                    $"Cannot update income: total budgets ({oldMonthBudgets:F2}) would exceed total income ({newOldMonthIncome:F2}) for {oldYear}-{oldMonth:D2}."));

            var oldMonthExpenses = await _expenses.GetTotalForMonthAsync(
                request.UserId, oldYear, oldMonth, ct);
            if (newOldMonthIncome < oldMonthExpenses)
                return Result.Failure(DomainError.Conflict(
                    $"Cannot update income: total expenses ({oldMonthExpenses:F2}) would exceed total income ({newOldMonthIncome:F2}) for {oldYear}-{oldMonth:D2}."));
        }

        income.Amount = request.Amount;
        income.Date = request.Date;
        income.IncomeCategoryId = request.IncomeCategoryId;

        _incomes.Update(income);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

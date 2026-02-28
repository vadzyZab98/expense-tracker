using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Incomes.DeleteIncome;

public sealed class DeleteIncomeCommandHandler
    : IRequestHandler<DeleteIncomeCommand, Result>
{
    private readonly IIncomeRepository _incomes;
    private readonly IExpenseRepository _expenses;
    private readonly IMonthlyBudgetRepository _budgets;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIncomeCommandHandler(
        IIncomeRepository incomes,
        IExpenseRepository expenses,
        IMonthlyBudgetRepository budgets,
        IUnitOfWork unitOfWork)
    {
        _incomes = incomes;
        _expenses = expenses;
        _budgets = budgets;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteIncomeCommand request, CancellationToken ct)
    {
        var income = await _incomes.FindByIdAndUserAsync(request.Id, request.UserId, ct);
        if (income is null)
            return Result.Failure(DomainError.NotFound("Income", request.Id));

        var year = income.Date.Year;
        var month = income.Date.Month;

        var totalIncome = await _incomes.GetTotalForMonthAsync(request.UserId, year, month, ct);
        var newTotalIncome = totalIncome - income.Amount;

        var totalBudgets = await _budgets.GetTotalForMonthAsync(request.UserId, year, month, ct);
        if (newTotalIncome < totalBudgets)
            return Result.Failure(DomainError.Conflict(
                $"Cannot delete income: total budgets ({totalBudgets:F2}) would exceed total income ({newTotalIncome:F2}) for {year}-{month:D2}."));

        var totalExpenses = await _expenses.GetTotalForMonthAsync(request.UserId, year, month, ct);
        if (newTotalIncome < totalExpenses)
            return Result.Failure(DomainError.Conflict(
                $"Cannot delete income: total expenses ({totalExpenses:F2}) would exceed total income ({newTotalIncome:F2}) for {year}-{month:D2}."));

        _incomes.Delete(income);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

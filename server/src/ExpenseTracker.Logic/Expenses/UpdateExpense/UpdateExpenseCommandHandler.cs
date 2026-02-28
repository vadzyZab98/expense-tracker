using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.UpdateExpense;

public sealed class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Result>
{
    private readonly IExpenseRepository _expenses;
    private readonly IIncomeRepository _incomes;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExpenseCommandHandler(
        IExpenseRepository expenses,
        IIncomeRepository incomes,
        IUnitOfWork unitOfWork)
    {
        _expenses = expenses;
        _incomes = incomes;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateExpenseCommand request, CancellationToken ct)
    {
        var expense = await _expenses.FindByIdAndUserAsync(request.Id, request.UserId, ct);
        if (expense is null)
            return Result.Failure(DomainError.NotFound("Expense", request.Id));

        var targetYear = request.Date.Year;
        var targetMonth = request.Date.Month;
        var oldYear = expense.Date.Year;
        var oldMonth = expense.Date.Month;
        var monthChanged = oldYear != targetYear || oldMonth != targetMonth;

        // Validate income constraint on the target month
        if (monthChanged || request.Amount > expense.Amount)
        {
            var totalIncome = await _incomes.GetTotalForMonthAsync(
                request.UserId, targetYear, targetMonth, ct);
            if (totalIncome == 0)
                return Result.Failure(DomainError.Conflict(
                    $"No income recorded for {targetYear}-{targetMonth:D2}. Expense update is forbidden when total income is zero."));

            var currentTotalExpenses = await _expenses.GetTotalForMonthAsync(
                request.UserId, targetYear, targetMonth, ct);
            var newTotalExpenses = monthChanged
                ? currentTotalExpenses + request.Amount
                : currentTotalExpenses - expense.Amount + request.Amount;

            if (newTotalExpenses > totalIncome)
                return Result.Failure(DomainError.Conflict(
                    $"Total expenses ({newTotalExpenses:F2}) would exceed total income ({totalIncome:F2}) for {targetYear}-{targetMonth:D2}."));
        }

        expense.Amount = request.Amount;
        expense.Description = request.Description;
        expense.Date = request.Date;
        expense.CategoryId = request.CategoryId;

        _expenses.Update(expense);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.CreateExpense;

public sealed class CreateExpenseCommandHandler
    : IRequestHandler<CreateExpenseCommand, Result<ExpenseResponse>>
{
    private readonly IExpenseRepository _expenses;
    private readonly ICategoryRepository _categories;
    private readonly IIncomeRepository _incomes;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExpenseCommandHandler(
        IExpenseRepository expenses,
        ICategoryRepository categories,
        IIncomeRepository incomes,
        IUnitOfWork unitOfWork)
    {
        _expenses = expenses;
        _categories = categories;
        _incomes = incomes;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExpenseResponse>> Handle(CreateExpenseCommand request, CancellationToken ct)
    {
        var category = await _categories.FindByIdAsync(request.CategoryId, ct);
        if (category is null)
            return Result<ExpenseResponse>.Failure(DomainError.NotFound("Category", request.CategoryId));

        var year = request.Date.Year;
        var month = request.Date.Month;

        var totalIncome = await _incomes.GetTotalForMonthAsync(request.UserId, year, month, ct);
        if (totalIncome == 0)
            return Result<ExpenseResponse>.Failure(
                DomainError.Conflict(
                    $"No income recorded for {year}-{month:D2}. Expense creation is forbidden when total income is zero."));

        var currentTotalExpenses = await _expenses.GetTotalForMonthAsync(request.UserId, year, month, ct);
        var newTotalExpenses = currentTotalExpenses + request.Amount;
        if (newTotalExpenses > totalIncome)
            return Result<ExpenseResponse>.Failure(
                DomainError.Conflict(
                    $"Total expenses ({newTotalExpenses:F2}) would exceed total income ({totalIncome:F2}) for {year}-{month:D2}."));

        var expense = new Expense
        {
            UserId = request.UserId,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Description = request.Description,
            Date = request.Date
        };

        await _expenses.AddAsync(expense, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ExpenseResponse>.Success(new ExpenseResponse(
            expense.Id,
            expense.Amount,
            expense.Description,
            expense.Date,
            expense.CategoryId,
            new CategoryResponse(category.Id, category.Name, category.Color)));
    }
}

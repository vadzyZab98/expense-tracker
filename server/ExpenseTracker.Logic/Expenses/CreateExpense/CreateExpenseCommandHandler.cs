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
    private readonly IUnitOfWork _unitOfWork;

    public CreateExpenseCommandHandler(
        IExpenseRepository expenses,
        ICategoryRepository categories,
        IUnitOfWork unitOfWork)
    {
        _expenses = expenses;
        _categories = categories;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExpenseResponse>> Handle(CreateExpenseCommand request, CancellationToken ct)
    {
        var category = await _categories.FindByIdAsync(request.CategoryId, ct);
        if (category is null)
            return Result<ExpenseResponse>.Failure(DomainError.NotFound("Category", request.CategoryId));

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

using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.GetExpenseById;

public sealed class GetExpenseByIdQueryHandler
    : IRequestHandler<GetExpenseByIdQuery, Result<ExpenseResponse>>
{
    private readonly IExpenseRepository _expenses;

    public GetExpenseByIdQueryHandler(IExpenseRepository expenses)
    {
        _expenses = expenses;
    }

    public async Task<Result<ExpenseResponse>> Handle(
        GetExpenseByIdQuery request, CancellationToken ct)
    {
        var expense = await _expenses.FindByIdAndUserAsync(request.Id, request.UserId, ct);
        if (expense is null)
            return Result<ExpenseResponse>.Failure(DomainError.NotFound("Expense", request.Id));

        return Result<ExpenseResponse>.Success(new ExpenseResponse(
            expense.Id,
            expense.Amount,
            expense.Description,
            expense.Date,
            expense.CategoryId,
            new CategoryResponse(expense.Category.Id, expense.Category.Name, expense.Category.Color)));
    }
}

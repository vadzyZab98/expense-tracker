using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.GetExpenses;

public sealed class GetExpensesQueryHandler
    : IRequestHandler<GetExpensesQuery, IReadOnlyList<ExpenseResponse>>
{
    private readonly IExpenseRepository _expenses;

    public GetExpensesQueryHandler(IExpenseRepository expenses)
    {
        _expenses = expenses;
    }

    public async Task<IReadOnlyList<ExpenseResponse>> Handle(
        GetExpensesQuery request, CancellationToken ct)
    {
        var expenses = await _expenses.GetByUserAsync(request.UserId, ct);

        return expenses
            .OrderByDescending(e => e.Date)
            .Select(e => new ExpenseResponse(
                e.Id,
                e.Amount,
                e.Description,
                e.Date,
                e.CategoryId,
                new CategoryResponse(e.Category.Id, e.Category.Name, e.Category.Color)))
            .ToList();
    }
}

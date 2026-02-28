using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Budgets.GetBudgetsByMonth;

public sealed class GetBudgetsByMonthQueryHandler
    : IRequestHandler<GetBudgetsByMonthQuery, IReadOnlyList<MonthlyBudgetResponse>>
{
    private readonly IMonthlyBudgetRepository _budgets;

    public GetBudgetsByMonthQueryHandler(IMonthlyBudgetRepository budgets)
    {
        _budgets = budgets;
    }

    public async Task<IReadOnlyList<MonthlyBudgetResponse>> Handle(
        GetBudgetsByMonthQuery request, CancellationToken ct)
    {
        var budgets = await _budgets.GetByUserAndMonthAsync(
            request.UserId, request.Year, request.Month, ct);

        return budgets
            .Select(b => new MonthlyBudgetResponse(
                b.Id,
                b.CategoryId,
                new CategoryResponse(b.Category.Id, b.Category.Name, b.Category.Color),
                b.Year,
                b.Month,
                b.Amount))
            .ToList();
    }
}

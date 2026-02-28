using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Budgets.GetBudgets;

public sealed class GetBudgetsQueryHandler
    : IRequestHandler<GetBudgetsQuery, IReadOnlyList<MonthlyBudgetResponse>>
{
    private readonly IMonthlyBudgetRepository _budgets;

    public GetBudgetsQueryHandler(IMonthlyBudgetRepository budgets)
    {
        _budgets = budgets;
    }

    public async Task<IReadOnlyList<MonthlyBudgetResponse>> Handle(
        GetBudgetsQuery request, CancellationToken ct)
    {
        var budgets = await _budgets.GetByUserAsync(request.UserId, ct);

        return budgets
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
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

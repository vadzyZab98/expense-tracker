using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Incomes.GetIncomes;

public sealed class GetIncomesQueryHandler
    : IRequestHandler<GetIncomesQuery, IReadOnlyList<IncomeResponse>>
{
    private readonly IIncomeRepository _incomes;

    public GetIncomesQueryHandler(IIncomeRepository incomes)
    {
        _incomes = incomes;
    }

    public async Task<IReadOnlyList<IncomeResponse>> Handle(
        GetIncomesQuery request, CancellationToken ct)
    {
        var incomes = await _incomes.GetByUserAsync(request.UserId, ct);

        return incomes
            .OrderByDescending(i => i.Date)
            .Select(i => new IncomeResponse(
                i.Id,
                i.Amount,
                i.Date,
                i.IncomeCategoryId,
                new IncomeCategoryResponse(
                    i.IncomeCategory.Id,
                    i.IncomeCategory.Name,
                    i.IncomeCategory.Color)))
            .ToList();
    }
}

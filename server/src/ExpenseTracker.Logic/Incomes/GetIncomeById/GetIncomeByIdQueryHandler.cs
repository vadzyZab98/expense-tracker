using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Incomes.GetIncomeById;

public sealed class GetIncomeByIdQueryHandler
    : IRequestHandler<GetIncomeByIdQuery, Result<IncomeResponse>>
{
    private readonly IIncomeRepository _incomes;

    public GetIncomeByIdQueryHandler(IIncomeRepository incomes)
    {
        _incomes = incomes;
    }

    public async Task<Result<IncomeResponse>> Handle(
        GetIncomeByIdQuery request, CancellationToken ct)
    {
        var income = await _incomes.FindByIdAndUserAsync(request.Id, request.UserId, ct);
        if (income is null)
            return Result<IncomeResponse>.Failure(DomainError.NotFound("Income", request.Id));

        return Result<IncomeResponse>.Success(new IncomeResponse(
            income.Id,
            income.Amount,
            income.Date,
            income.IncomeCategoryId,
            new IncomeCategoryResponse(
                income.IncomeCategory.Id,
                income.IncomeCategory.Name,
                income.IncomeCategory.Color)));
    }
}

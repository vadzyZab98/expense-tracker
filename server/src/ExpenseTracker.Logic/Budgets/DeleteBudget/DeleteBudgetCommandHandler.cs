using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Budgets.DeleteBudget;

public sealed class DeleteBudgetCommandHandler
    : IRequestHandler<DeleteBudgetCommand, Result>
{
    private readonly IMonthlyBudgetRepository _budgets;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBudgetCommandHandler(
        IMonthlyBudgetRepository budgets,
        IUnitOfWork unitOfWork)
    {
        _budgets = budgets;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteBudgetCommand request, CancellationToken ct)
    {
        var budget = await _budgets.FindByIdAndUserAsync(request.Id, request.UserId, ct);
        if (budget is null)
            return Result.Failure(DomainError.NotFound("MonthlyBudget", request.Id));

        _budgets.Delete(budget);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

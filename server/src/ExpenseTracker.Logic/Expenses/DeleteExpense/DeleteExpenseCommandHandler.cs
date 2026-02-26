using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.DeleteExpense;

public sealed class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, Result>
{
    private readonly IExpenseRepository _expenses;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExpenseCommandHandler(IExpenseRepository expenses, IUnitOfWork unitOfWork)
    {
        _expenses = expenses;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteExpenseCommand request, CancellationToken ct)
    {
        var expense = await _expenses.FindByIdAndUserAsync(request.Id, request.UserId, ct);
        if (expense is null)
            return Result.Failure(DomainError.NotFound("Expense", request.Id));

        _expenses.Delete(expense);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

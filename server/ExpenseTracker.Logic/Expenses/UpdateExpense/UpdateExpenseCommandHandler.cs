using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Expenses.UpdateExpense;

public sealed class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Result>
{
    private readonly IExpenseRepository _expenses;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExpenseCommandHandler(IExpenseRepository expenses, IUnitOfWork unitOfWork)
    {
        _expenses = expenses;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateExpenseCommand request, CancellationToken ct)
    {
        var expense = await _expenses.FindByIdAndUserAsync(request.Id, request.UserId, ct);
        if (expense is null)
            return Result.Failure(DomainError.NotFound("Expense", request.Id));

        expense.Amount = request.Amount;
        expense.Description = request.Description;
        expense.Date = request.Date;
        expense.CategoryId = request.CategoryId;

        _expenses.Update(expense);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

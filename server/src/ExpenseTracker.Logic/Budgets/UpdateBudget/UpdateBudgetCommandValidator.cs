using FluentValidation;

namespace ExpenseTracker.Logic.Budgets.UpdateBudget;

public sealed class UpdateBudgetCommandValidator : AbstractValidator<UpdateBudgetCommand>
{
    public UpdateBudgetCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0);

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100);

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12);

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}

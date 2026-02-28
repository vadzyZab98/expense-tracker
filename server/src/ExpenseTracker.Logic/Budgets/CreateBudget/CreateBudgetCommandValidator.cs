using FluentValidation;

namespace ExpenseTracker.Logic.Budgets.CreateBudget;

public sealed class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
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

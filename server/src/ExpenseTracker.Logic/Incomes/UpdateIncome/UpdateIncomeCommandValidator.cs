using FluentValidation;

namespace ExpenseTracker.Logic.Incomes.UpdateIncome;

public sealed class UpdateIncomeCommandValidator : AbstractValidator<UpdateIncomeCommand>
{
    public UpdateIncomeCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.IncomeCategoryId)
            .GreaterThan(0);
    }
}

using FluentValidation;

namespace ExpenseTracker.Logic.Incomes.CreateIncome;

public sealed class CreateIncomeCommandValidator : AbstractValidator<CreateIncomeCommand>
{
    public CreateIncomeCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.IncomeCategoryId)
            .GreaterThan(0);
    }
}

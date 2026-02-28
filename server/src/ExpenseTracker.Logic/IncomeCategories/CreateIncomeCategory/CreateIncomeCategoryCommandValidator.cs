using FluentValidation;

namespace ExpenseTracker.Logic.IncomeCategories.CreateIncomeCategory;

public sealed class CreateIncomeCategoryCommandValidator
    : AbstractValidator<CreateIncomeCategoryCommand>
{
    public CreateIncomeCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Color)
            .NotEmpty()
            .MaximumLength(7);
    }
}

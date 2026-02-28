using FluentValidation;

namespace ExpenseTracker.Logic.IncomeCategories.UpdateIncomeCategory;

public sealed class UpdateIncomeCategoryCommandValidator
    : AbstractValidator<UpdateIncomeCategoryCommand>
{
    public UpdateIncomeCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Color)
            .NotEmpty()
            .MaximumLength(7);
    }
}

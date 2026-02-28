using FluentValidation.TestHelper;
using ExpenseTracker.Logic.Budgets.CreateBudget;

namespace ExpenseTracker.UnitTests.Logic.Budgets.Commands;

public sealed class CreateBudgetCommandValidatorTests
{
    private readonly CreateBudgetCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_WhenValid()
    {
        var command = new CreateBudgetCommand(1, 1, 2025, 6, 500m);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Fail_WhenCategoryIdNotPositive(int categoryId)
    {
        var command = new CreateBudgetCommand(1, categoryId, 2025, 6, 500m);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Theory]
    [InlineData(1999)]
    [InlineData(2101)]
    public void Should_Fail_WhenYearOutOfRange(int year)
    {
        var command = new CreateBudgetCommand(1, 1, year, 6, 500m);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Should_Fail_WhenMonthOutOfRange(int month)
    {
        var command = new CreateBudgetCommand(1, 1, 2025, month, 500m);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Month);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Fail_WhenAmountNotPositive(decimal amount)
    {
        var command = new CreateBudgetCommand(1, 1, 2025, 6, amount);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }
}

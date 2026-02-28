using FluentValidation.TestHelper;
using ExpenseTracker.Logic.Incomes.CreateIncome;

namespace ExpenseTracker.UnitTests.Logic.Incomes.Commands;

public sealed class CreateIncomeCommandValidatorTests
{
    private readonly CreateIncomeCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_WhenValid()
    {
        var command = new CreateIncomeCommand(1, 5000m, new DateTime(2025, 6, 15), 1);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Fail_WhenAmountNotPositive(decimal amount)
    {
        var command = new CreateIncomeCommand(1, amount, new DateTime(2025, 6, 15), 1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_Fail_WhenDateIsEmpty()
    {
        var command = new CreateIncomeCommand(1, 5000m, default, 1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Fail_WhenIncomeCategoryIdNotPositive(int categoryId)
    {
        var command = new CreateIncomeCommand(1, 5000m, new DateTime(2025, 6, 15), categoryId);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.IncomeCategoryId);
    }
}

using FluentValidation.TestHelper;
using ExpenseTracker.Logic.IncomeCategories.CreateIncomeCategory;

namespace ExpenseTracker.UnitTests.Logic.IncomeCategories.Commands;

public sealed class CreateIncomeCategoryCommandValidatorTests
{
    private readonly CreateIncomeCategoryCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_WhenValid()
    {
        var command = new CreateIncomeCategoryCommand("Salary", "#4CAF50");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Fail_WhenNameIsEmpty(string? name)
    {
        var command = new CreateIncomeCategoryCommand(name!, "#4CAF50");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Fail_WhenNameTooLong()
    {
        var command = new CreateIncomeCategoryCommand(new string('A', 101), "#4CAF50");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Fail_WhenColorIsEmpty(string? color)
    {
        var command = new CreateIncomeCategoryCommand("Salary", color!);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Fact]
    public void Should_Fail_WhenColorTooLong()
    {
        var command = new CreateIncomeCategoryCommand("Salary", "#1234567");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }
}

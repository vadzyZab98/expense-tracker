using FluentValidation.TestHelper;
using ExpenseTracker.Logic.Expenses.UpdateExpense;

namespace ExpenseTracker.UnitTests.Logic.Expenses.Commands;

public sealed class UpdateExpenseCommandValidatorTests
{
    private readonly UpdateExpenseCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Id_WhenNotPositive_ShouldReturnError(int value)
    {
        // Arrange
        var command = new UpdateExpenseCommand(value, 1, 10m, "Lunch", DateTime.Now, 1);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Amount_WhenNotPositive_ShouldReturnError(decimal value)
    {
        // Arrange
        var command = new UpdateExpenseCommand(1, 1, value, "Lunch", DateTime.Now, 1);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Description_WhenEmpty_ShouldReturnError(string? value)
    {
        // Arrange
        var command = new UpdateExpenseCommand(1, 1, 10m, value!, DateTime.Now, 1);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task Description_WhenTooLong_ShouldReturnError()
    {
        // Arrange
        var command = new UpdateExpenseCommand(1, 1, 10m, new string('A', 501), DateTime.Now, 1);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CategoryId_WhenNotPositive_ShouldReturnError(int value)
    {
        // Arrange
        var command = new UpdateExpenseCommand(1, 1, 10m, "Lunch", DateTime.Now, value);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public async Task ValidModel_ShouldReturnNoError()
    {
        // Arrange
        var command = new UpdateExpenseCommand(1, 1, 42.50m, "Lunch", DateTime.Now, 1);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

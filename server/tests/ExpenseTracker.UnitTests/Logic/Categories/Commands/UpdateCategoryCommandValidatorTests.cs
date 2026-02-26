using FluentValidation.TestHelper;
using ExpenseTracker.Logic.Categories.UpdateCategory;

namespace ExpenseTracker.UnitTests.Logic.Categories.Commands;

public sealed class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Id_WhenNotPositive_ShouldReturnError(int value)
    {
        // Arrange
        var command = new UpdateCategoryCommand(value, "Food", "#FF6B6B");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Name_WhenEmpty_ShouldReturnError(string? value)
    {
        // Arrange
        var command = new UpdateCategoryCommand(1, value!, "#FF6B6B");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("FF6B6B")]
    [InlineData("#GGGGGG")]
    public async Task Color_WhenInvalidHex_ShouldReturnError(string value)
    {
        // Arrange
        var command = new UpdateCategoryCommand(1, "Food", value);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Fact]
    public async Task ValidModel_ShouldReturnNoError()
    {
        // Arrange
        var command = new UpdateCategoryCommand(1, "Food", "#FF6B6B");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

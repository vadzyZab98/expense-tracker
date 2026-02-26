using FluentValidation.TestHelper;
using ExpenseTracker.Logic.Categories.CreateCategory;

namespace ExpenseTracker.UnitTests.Logic.Categories.Commands;

public sealed class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Name_WhenEmpty_ShouldReturnError(string? value)
    {
        // Arrange
        var command = new CreateCategoryCommand(value!, "#FF6B6B");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Name_WhenTooLong_ShouldReturnError()
    {
        // Arrange
        var command = new CreateCategoryCommand(new string('A', 101), "#FF6B6B");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Color_WhenEmpty_ShouldReturnError(string? value)
    {
        // Arrange
        var command = new CreateCategoryCommand("Food", value!);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("FF6B6B")]
    [InlineData("#FF6B6")]
    [InlineData("#GGGGGG")]
    public async Task Color_WhenInvalidHex_ShouldReturnError(string value)
    {
        // Arrange
        var command = new CreateCategoryCommand("Food", value);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Theory]
    [InlineData("#FF6B6B")]
    [InlineData("#000000")]
    [InlineData("#abcdef")]
    public async Task Color_WhenValidHex_ShouldReturnNoError(string value)
    {
        // Arrange
        var command = new CreateCategoryCommand("Food", value);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Color);
    }

    [Fact]
    public async Task ValidModel_ShouldReturnNoError()
    {
        // Arrange
        var command = new CreateCategoryCommand("Food", "#FF6B6B");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

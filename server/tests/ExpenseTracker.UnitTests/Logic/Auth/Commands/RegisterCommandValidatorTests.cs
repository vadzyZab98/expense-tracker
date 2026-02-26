using FluentValidation.TestHelper;
using ExpenseTracker.Logic.Auth.Register;

namespace ExpenseTracker.UnitTests.Logic.Auth.Commands;

public sealed class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Email_WhenEmpty_ShouldReturnError(string? value)
    {
        // Arrange
        var command = new RegisterCommand(value!, "Password1");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing-at.com")]
    public async Task Email_WhenInvalidFormat_ShouldReturnError(string value)
    {
        // Arrange
        var command = new RegisterCommand(value, "Password1");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Password_WhenEmpty_ShouldReturnError(string? value)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", value!);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Password_WhenTooShort_ShouldReturnError()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Pass1");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Password_WhenNoDigit_ShouldReturnError()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "PasswordWithoutDigit");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one digit.");
    }

    [Fact]
    public async Task ValidModel_ShouldReturnNoError()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password1");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

using FluentValidation.TestHelper;
using ExpenseTracker.Logic.Auth.Login;

namespace ExpenseTracker.UnitTests.Logic.Auth.Queries;

public sealed class LoginQueryValidatorTests
{
    private readonly LoginQueryValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Email_WhenEmpty_ShouldReturnError(string? value)
    {
        // Arrange
        var query = new LoginQuery(value!, "password");

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Email_WhenInvalidFormat_ShouldReturnError()
    {
        // Arrange
        var query = new LoginQuery("not-an-email", "password");

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Password_WhenEmpty_ShouldReturnError(string? value)
    {
        // Arrange
        var query = new LoginQuery("test@example.com", value!);

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task ValidModel_ShouldReturnNoError()
    {
        // Arrange
        var query = new LoginQuery("test@example.com", "any-password");

        // Act
        var result = await _validator.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

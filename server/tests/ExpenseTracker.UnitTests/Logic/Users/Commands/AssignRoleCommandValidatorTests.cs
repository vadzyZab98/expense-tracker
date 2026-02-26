using FluentValidation.TestHelper;
using ExpenseTracker.Logic.Users.AssignRole;

namespace ExpenseTracker.UnitTests.Logic.Users.Commands;

public sealed class AssignRoleCommandValidatorTests
{
    private readonly AssignRoleCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UserId_WhenNotPositive_ShouldReturnError(int value)
    {
        // Arrange
        var command = new AssignRoleCommand(value, "Admin");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Role_WhenEmpty_ShouldReturnError(string? value)
    {
        // Arrange
        var command = new AssignRoleCommand(1, value!);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("Moderator")]
    [InlineData("Guest")]
    public async Task Role_WhenNotAllowed_ShouldReturnError(string value)
    {
        // Arrange
        var command = new AssignRoleCommand(1, value);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role)
            .WithErrorMessage("Role must be 'User' or 'Admin'.");
    }

    [Theory]
    [InlineData("User")]
    [InlineData("Admin")]
    public async Task Role_WhenAllowed_ShouldReturnNoError(string value)
    {
        // Arrange
        var command = new AssignRoleCommand(1, value);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public async Task ValidModel_ShouldReturnNoError()
    {
        // Arrange
        var command = new AssignRoleCommand(1, "Admin");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

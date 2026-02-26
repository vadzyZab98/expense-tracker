using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Auth.Login;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Interfaces;

namespace ExpenseTracker.UnitTests.Logic.Auth.Queries;

public sealed class LoginQueryHandlerTests
{
    private readonly Mock<IUserRepository> _users;
    private readonly Mock<IPasswordService> _passwords;
    private readonly Mock<ITokenService> _tokens;
    private readonly LoginQueryHandler _handler;

    public LoginQueryHandlerTests()
    {
        _users = new Mock<IUserRepository>();
        _passwords = new Mock<IPasswordService>();
        _tokens = new Mock<ITokenService>();
        _handler = new LoginQueryHandler(_users.Object, _passwords.Object, _tokens.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@example.com", PasswordHash = "hash", Role = "User" };
        _users.Setup(x => x.FindByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwords.Setup(x => x.Verify("Password123", "hash"))
            .Returns(true);
        _tokens.Setup(x => x.GenerateToken(user))
            .Returns("jwt-token");

        var query = new LoginQuery("test@example.com", "Password123");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().Be("jwt-token");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        _users.Setup(x => x.FindByEmailAsync("unknown@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var query = new LoginQuery("unknown@example.com", "Password123");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Unauthorized);
        result.Error!.Message.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@example.com", PasswordHash = "hash", Role = "User" };
        _users.Setup(x => x.FindByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwords.Setup(x => x.Verify("WrongPassword", "hash"))
            .Returns(false);

        var query = new LoginQuery("test@example.com", "WrongPassword");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Unauthorized);
    }

    [Fact]
    public async Task Handle_WrongPassword_DoesNotGenerateToken()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@example.com", PasswordHash = "hash", Role = "User" };
        _users.Setup(x => x.FindByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwords.Setup(x => x.Verify("WrongPassword", "hash"))
            .Returns(false);

        var query = new LoginQuery("test@example.com", "WrongPassword");

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _tokens.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }
}

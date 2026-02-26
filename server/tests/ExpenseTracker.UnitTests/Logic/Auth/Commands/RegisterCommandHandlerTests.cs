using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Auth.Register;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Interfaces;

namespace ExpenseTracker.UnitTests.Logic.Auth.Commands;

public sealed class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _users;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IPasswordService> _passwords;
    private readonly Mock<ITokenService> _tokens;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _users = new Mock<IUserRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _passwords = new Mock<IPasswordService>();
        _tokens = new Mock<ITokenService>();
        _handler = new RegisterCommandHandler(
            _users.Object, _unitOfWork.Object, _passwords.Object, _tokens.Object);
    }

    [Fact]
    public async Task Handle_ValidNewUser_ReturnsToken()
    {
        // Arrange
        _users.Setup(x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwords.Setup(x => x.Hash(It.IsAny<string>()))
            .Returns("hashed-password");
        _tokens.Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns("jwt-token");

        var command = new RegisterCommand("test@example.com", "Password123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().Be("jwt-token");
    }

    [Fact]
    public async Task Handle_ValidNewUser_SavesUserWithHashedPassword()
    {
        // Arrange
        _users.Setup(x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwords.Setup(x => x.Hash("Password123"))
            .Returns("hashed-password");
        _tokens.Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns("jwt-token");

        var command = new RegisterCommand("test@example.com", "Password123");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _users.Verify(x => x.AddAsync(
            It.Is<User>(u =>
                u.Email == "test@example.com" &&
                u.PasswordHash == "hashed-password" &&
                u.Role == "User"),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailAlreadyInUse_ReturnsConflict()
    {
        // Arrange
        _users.Setup(x => x.FindByEmailAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Email = "existing@example.com" });

        var command = new RegisterCommand("existing@example.com", "Password123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Be("Email already in use.");
    }

    [Fact]
    public async Task Handle_EmailAlreadyInUse_DoesNotSave()
    {
        // Arrange
        _users.Setup(x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Email = "existing@example.com" });

        var command = new RegisterCommand("existing@example.com", "Password123");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _users.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

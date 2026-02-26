using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Users.AssignRole;

namespace ExpenseTracker.UnitTests.Logic.Users.Commands;

public sealed class AssignRoleCommandHandlerTests
{
    private readonly Mock<IUserRepository> _users;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly AssignRoleCommandHandler _handler;

    public AssignRoleCommandHandlerTests()
    {
        _users = new Mock<IUserRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new AssignRoleCommandHandler(_users.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Id = 2, Email = "user@mail.com", Role = "User" };
        _users.Setup(x => x.FindByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new AssignRoleCommand(2, "Admin");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task Handle_ValidUser_SavesChanges()
    {
        // Arrange
        var user = new User { Id = 2, Email = "user@mail.com", Role = "User" };
        _users.Setup(x => x.FindByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new AssignRoleCommand(2, "Admin");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        _users.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new AssignRoleCommand(99, "Admin");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_SuperAdminUser_ReturnsConflict()
    {
        // Arrange
        var user = new User { Id = 1, Email = "admin@mail.ru", Role = "SuperAdmin" };
        _users.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new AssignRoleCommand(1, "User");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("SuperAdmin");
    }

    [Fact]
    public async Task Handle_SuperAdminUser_DoesNotSave()
    {
        // Arrange
        var user = new User { Id = 1, Email = "admin@mail.ru", Role = "SuperAdmin" };
        _users.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new AssignRoleCommand(1, "User");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RevokeAdmin_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Id = 3, Email = "admin-user@mail.com", Role = "Admin" };
        _users.Setup(x => x.FindByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new AssignRoleCommand(3, "User");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Role.Should().Be("User");
    }
}

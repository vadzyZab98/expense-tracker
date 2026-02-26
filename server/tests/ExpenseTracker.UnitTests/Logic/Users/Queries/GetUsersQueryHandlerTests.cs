using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Users.GetUsers;

namespace ExpenseTracker.UnitTests.Logic.Users.Queries;

public sealed class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _users;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _users = new Mock<IUserRepository>();
        _handler = new GetUsersQueryHandler(_users.Object);
    }

    [Fact]
    public async Task Handle_UsersExist_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Email = "admin@mail.ru", Role = "SuperAdmin" },
            new() { Id = 2, Email = "user@mail.com", Role = "User" }
        };
        _users.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var query = new GetUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Email.Should().Be("admin@mail.ru");
        result[0].Role.Should().Be("SuperAdmin");
        result[1].Email.Should().Be("user@mail.com");
        result[1].Role.Should().Be("User");
    }

    [Fact]
    public async Task Handle_NoUsers_ReturnsEmptyList()
    {
        // Arrange
        _users.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        var query = new GetUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}

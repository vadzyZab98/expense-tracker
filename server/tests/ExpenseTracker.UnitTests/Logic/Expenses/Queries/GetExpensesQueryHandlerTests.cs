using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Expenses.GetExpenses;

namespace ExpenseTracker.UnitTests.Logic.Expenses.Queries;

public sealed class GetExpensesQueryHandlerTests
{
    private readonly Mock<IExpenseRepository> _expenses;
    private readonly GetExpensesQueryHandler _handler;

    public GetExpensesQueryHandlerTests()
    {
        _expenses = new Mock<IExpenseRepository>();
        _handler = new GetExpensesQueryHandler(_expenses.Object);
    }

    [Fact]
    public async Task Handle_ExpensesExist_ReturnsSortedByDateDescending()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        var expenses = new List<Expense>
        {
            new() { Id = 1, UserId = 1, Amount = 10m, Description = "Old", Date = new DateTime(2025, 1, 1), CategoryId = 1, Category = category },
            new() { Id = 2, UserId = 1, Amount = 20m, Description = "New", Date = new DateTime(2025, 6, 15), CategoryId = 1, Category = category },
            new() { Id = 3, UserId = 1, Amount = 30m, Description = "Mid", Date = new DateTime(2025, 3, 10), CategoryId = 1, Category = category }
        };
        _expenses.Setup(x => x.GetByUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);

        var query = new GetExpensesQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].Description.Should().Be("New");
        result[1].Description.Should().Be("Mid");
        result[2].Description.Should().Be("Old");
    }

    [Fact]
    public async Task Handle_NoExpenses_ReturnsEmptyList()
    {
        // Arrange
        _expenses.Setup(x => x.GetByUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Expense>());

        var query = new GetExpensesQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ExpensesExist_MapsToExpenseResponse()
    {
        // Arrange
        var category = new Category { Id = 2, Name = "Transport", Color = "#0000FF" };
        var date = new DateTime(2025, 6, 15);
        var expenses = new List<Expense>
        {
            new() { Id = 5, UserId = 1, Amount = 50m, Description = "Taxi", Date = date, CategoryId = 2, Category = category }
        };
        _expenses.Setup(x => x.GetByUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);

        var query = new GetExpensesQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var expense = result.Should().ContainSingle().Subject;
        expense.Id.Should().Be(5);
        expense.Amount.Should().Be(50m);
        expense.Description.Should().Be("Taxi");
        expense.Date.Should().Be(date);
        expense.CategoryId.Should().Be(2);
        expense.Category.Name.Should().Be("Transport");
        expense.Category.Color.Should().Be("#0000FF");
    }
}

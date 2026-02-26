using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Expenses.GetExpenseById;

namespace ExpenseTracker.UnitTests.Logic.Expenses.Queries;

public sealed class GetExpenseByIdQueryHandlerTests
{
    private readonly Mock<IExpenseRepository> _expenses;
    private readonly GetExpenseByIdQueryHandler _handler;

    public GetExpenseByIdQueryHandlerTests()
    {
        _expenses = new Mock<IExpenseRepository>();
        _handler = new GetExpenseByIdQueryHandler(_expenses.Object);
    }

    [Fact]
    public async Task Handle_ExistingExpense_ReturnsSuccess()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        var date = new DateTime(2025, 6, 15);
        var expense = new Expense
        {
            Id = 1, UserId = 1, Amount = 42.50m, Description = "Lunch",
            Date = date, CategoryId = 1, Category = category
        };
        _expenses.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);

        var query = new GetExpenseByIdQuery(1, 1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(1);
        result.Value!.Amount.Should().Be(42.50m);
        result.Value!.Description.Should().Be("Lunch");
        result.Value!.Category.Name.Should().Be("Food");
    }

    [Fact]
    public async Task Handle_NonExistentExpense_ReturnsNotFound()
    {
        // Arrange
        _expenses.Setup(x => x.FindByIdAndUserAsync(99, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense?)null);

        var query = new GetExpenseByIdQuery(99, 1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Expenses.CreateExpense;

namespace ExpenseTracker.UnitTests.Logic.Expenses.Commands;

public sealed class CreateExpenseCommandHandlerTests
{
    private readonly Mock<IExpenseRepository> _expenses;
    private readonly Mock<ICategoryRepository> _categories;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateExpenseCommandHandler _handler;

    public CreateExpenseCommandHandlerTests()
    {
        _expenses = new Mock<IExpenseRepository>();
        _categories = new Mock<ICategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateExpenseCommandHandler(
            _expenses.Object, _categories.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithExpense()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var date = new DateTime(2025, 6, 15);
        var command = new CreateExpenseCommand(1, 42.50m, "Lunch", date, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(42.50m);
        result.Value!.Description.Should().Be("Lunch");
        result.Value!.Date.Should().Be(date);
        result.Value!.CategoryId.Should().Be(1);
        result.Value!.Category.Name.Should().Be("Food");
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsExpense()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var date = new DateTime(2025, 6, 15);
        var command = new CreateExpenseCommand(1, 42.50m, "Lunch", date, 1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _expenses.Verify(x => x.AddAsync(
            It.Is<Expense>(e =>
                e.UserId == 1 &&
                e.Amount == 42.50m &&
                e.Description == "Lunch" &&
                e.CategoryId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        _categories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var command = new CreateExpenseCommand(1, 42.50m, "Lunch", DateTime.Now, 99);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_DoesNotSave()
    {
        // Arrange
        _categories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var command = new CreateExpenseCommand(1, 42.50m, "Lunch", DateTime.Now, 99);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _expenses.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

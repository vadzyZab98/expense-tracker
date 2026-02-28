using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Expenses.UpdateExpense;

namespace ExpenseTracker.UnitTests.Logic.Expenses.Commands;

public sealed class UpdateExpenseCommandHandlerTests
{
    private readonly Mock<IExpenseRepository> _expenses;
    private readonly Mock<IIncomeRepository> _incomes;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UpdateExpenseCommandHandler _handler;

    public UpdateExpenseCommandHandlerTests()
    {
        _expenses = new Mock<IExpenseRepository>();
        _incomes = new Mock<IIncomeRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new UpdateExpenseCommandHandler(
            _expenses.Object, _incomes.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingExpense_ReturnsSuccess()
    {
        // Arrange
        var expense = new Expense
        {
            Id = 1, UserId = 1, Amount = 10m, Description = "Old",
            Date = new DateTime(2025, 7, 1), CategoryId = 1
        };
        _expenses.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);
        _expenses.Setup(x => x.GetTotalForMonthAsync(1, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10m);

        var command = new UpdateExpenseCommand(1, 1, 25.00m, "Updated", new DateTime(2025, 7, 1), 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ExistingExpense_UpdatesAllFields()
    {
        // Arrange
        var expense = new Expense
        {
            Id = 1, UserId = 1, Amount = 10m, Description = "Old",
            Date = new DateTime(2025, 7, 1), CategoryId = 1
        };
        _expenses.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);
        _expenses.Setup(x => x.GetTotalForMonthAsync(1, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10m);

        var newDate = new DateTime(2025, 7, 1);
        var command = new UpdateExpenseCommand(1, 1, 25.00m, "Updated", newDate, 2);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        expense.Amount.Should().Be(25.00m);
        expense.Description.Should().Be("Updated");
        expense.Date.Should().Be(newDate);
        expense.CategoryId.Should().Be(2);
        _expenses.Verify(x => x.Update(expense), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentExpense_ReturnsNotFound()
    {
        // Arrange
        _expenses.Setup(x => x.FindByIdAndUserAsync(99, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense?)null);

        var command = new UpdateExpenseCommand(99, 1, 25.00m, "Updated", DateTime.Now, 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_NonExistentExpense_DoesNotUpdate()
    {
        // Arrange
        _expenses.Setup(x => x.FindByIdAndUserAsync(99, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense?)null);

        var command = new UpdateExpenseCommand(99, 1, 25.00m, "Updated", DateTime.Now, 2);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _expenses.Verify(x => x.Update(It.IsAny<Expense>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ZeroIncomeOnTargetMonth_ReturnsConflict()
    {
        // Arrange
        var expense = new Expense
        {
            Id = 1, UserId = 1, Amount = 50m, Description = "Old",
            Date = new DateTime(2025, 6, 1), CategoryId = 1
        };
        _expenses.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);
        // Moving to month with zero income
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var command = new UpdateExpenseCommand(1, 1, 50m, "Moved", new DateTime(2025, 8, 1), 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("No income recorded");
    }

    [Fact]
    public async Task Handle_IncreasedAmountWouldExceedIncome_ReturnsConflict()
    {
        // Arrange
        var expense = new Expense
        {
            Id = 1, UserId = 1, Amount = 100m, Description = "Old",
            Date = new DateTime(2025, 7, 1), CategoryId = 1
        };
        _expenses.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);
        _expenses.Setup(x => x.GetTotalForMonthAsync(1, 2025, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(950m);

        // Increasing from 100 to 500: net increase 400, current total 950 → 950-100+500 = 1350 > 1000
        var command = new UpdateExpenseCommand(1, 1, 500m, "Expensive", new DateTime(2025, 7, 1), 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("exceed total income");
    }
}

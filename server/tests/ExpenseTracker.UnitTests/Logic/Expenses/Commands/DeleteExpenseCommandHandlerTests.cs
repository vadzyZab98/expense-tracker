using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Expenses.DeleteExpense;

namespace ExpenseTracker.UnitTests.Logic.Expenses.Commands;

public sealed class DeleteExpenseCommandHandlerTests
{
    private readonly Mock<IExpenseRepository> _expenses;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly DeleteExpenseCommandHandler _handler;

    public DeleteExpenseCommandHandlerTests()
    {
        _expenses = new Mock<IExpenseRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteExpenseCommandHandler(_expenses.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingExpense_ReturnsSuccess()
    {
        // Arrange
        var expense = new Expense { Id = 1, UserId = 1 };
        _expenses.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);

        var command = new DeleteExpenseCommand(1, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _expenses.Verify(x => x.Delete(expense), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentExpense_ReturnsNotFound()
    {
        // Arrange
        _expenses.Setup(x => x.FindByIdAndUserAsync(99, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense?)null);

        var command = new DeleteExpenseCommand(99, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_NonExistentExpense_DoesNotDelete()
    {
        // Arrange
        _expenses.Setup(x => x.FindByIdAndUserAsync(99, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense?)null);

        var command = new DeleteExpenseCommand(99, 1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _expenses.Verify(x => x.Delete(It.IsAny<Expense>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

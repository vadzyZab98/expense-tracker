using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Budgets.DeleteBudget;
using ExpenseTracker.Logic.Common;

namespace ExpenseTracker.UnitTests.Logic.Budgets.Commands;

public sealed class DeleteBudgetCommandHandlerTests
{
    private readonly Mock<IMonthlyBudgetRepository> _budgets;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly DeleteBudgetCommandHandler _handler;

    public DeleteBudgetCommandHandlerTests()
    {
        _budgets = new Mock<IMonthlyBudgetRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteBudgetCommandHandler(_budgets.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingBudget_ReturnsSuccess()
    {
        var budget = new MonthlyBudget
        {
            Id = 1, UserId = 1, CategoryId = 1, Year = 2025, Month = 6, Amount = 500m
        };
        _budgets.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(budget);

        var result = await _handler.Handle(new DeleteBudgetCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _budgets.Verify(x => x.Delete(budget), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentBudget_ReturnsNotFound()
    {
        _budgets.Setup(x => x.FindByIdAndUserAsync(99, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MonthlyBudget?)null);

        var result = await _handler.Handle(new DeleteBudgetCommand(99, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

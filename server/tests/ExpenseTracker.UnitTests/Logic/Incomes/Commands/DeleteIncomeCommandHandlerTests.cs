using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Incomes.DeleteIncome;

namespace ExpenseTracker.UnitTests.Logic.Incomes.Commands;

public sealed class DeleteIncomeCommandHandlerTests
{
    private readonly Mock<IIncomeRepository> _incomes;
    private readonly Mock<IExpenseRepository> _expenses;
    private readonly Mock<IMonthlyBudgetRepository> _budgets;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly DeleteIncomeCommandHandler _handler;

    public DeleteIncomeCommandHandlerTests()
    {
        _incomes = new Mock<IIncomeRepository>();
        _expenses = new Mock<IExpenseRepository>();
        _budgets = new Mock<IMonthlyBudgetRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteIncomeCommandHandler(
            _incomes.Object, _expenses.Object, _budgets.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_NoBudgetsOrExpenses_ReturnsSuccess()
    {
        var income = new Income
        {
            Id = 1, UserId = 1, Amount = 5000m,
            Date = new DateTime(2025, 6, 15), IncomeCategoryId = 1
        };
        _incomes.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);
        _expenses.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var result = await _handler.Handle(new DeleteIncomeCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _incomes.Verify(x => x.Delete(income), Times.Once);
    }

    [Fact]
    public async Task Handle_WouldExceedBudgets_ReturnsConflict()
    {
        var income = new Income
        {
            Id = 1, UserId = 1, Amount = 5000m,
            Date = new DateTime(2025, 6, 15), IncomeCategoryId = 1
        };
        _incomes.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3000m);

        var result = await _handler.Handle(new DeleteIncomeCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("budgets");
    }

    [Fact]
    public async Task Handle_WouldExceedExpenses_ReturnsConflict()
    {
        var income = new Income
        {
            Id = 1, UserId = 1, Amount = 5000m,
            Date = new DateTime(2025, 6, 15), IncomeCategoryId = 1
        };
        _incomes.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);
        _expenses.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3000m);

        var result = await _handler.Handle(new DeleteIncomeCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("expenses");
    }

    [Fact]
    public async Task Handle_NonExistentIncome_ReturnsNotFound()
    {
        _incomes.Setup(x => x.FindByIdAndUserAsync(99, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Income?)null);

        var result = await _handler.Handle(new DeleteIncomeCommand(99, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

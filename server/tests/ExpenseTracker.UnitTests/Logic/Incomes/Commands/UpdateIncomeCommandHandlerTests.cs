using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Incomes.UpdateIncome;

namespace ExpenseTracker.UnitTests.Logic.Incomes.Commands;

public sealed class UpdateIncomeCommandHandlerTests
{
    private readonly Mock<IIncomeRepository> _incomes;
    private readonly Mock<IIncomeCategoryRepository> _incomeCategories;
    private readonly Mock<IExpenseRepository> _expenses;
    private readonly Mock<IMonthlyBudgetRepository> _budgets;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UpdateIncomeCommandHandler _handler;

    public UpdateIncomeCommandHandlerTests()
    {
        _incomes = new Mock<IIncomeRepository>();
        _incomeCategories = new Mock<IIncomeCategoryRepository>();
        _expenses = new Mock<IExpenseRepository>();
        _budgets = new Mock<IMonthlyBudgetRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new UpdateIncomeCommandHandler(
            _incomes.Object, _incomeCategories.Object,
            _expenses.Object, _budgets.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_IncreaseAmount_ReturnsSuccess()
    {
        var income = new Income
        {
            Id = 1, UserId = 1, Amount = 3000m,
            Date = new DateTime(2025, 6, 15), IncomeCategoryId = 1
        };
        _incomes.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);
        var category = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" };
        _incomeCategories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var command = new UpdateIncomeCommand(1, 1, 5000m, new DateTime(2025, 6, 15), 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        income.Amount.Should().Be(5000m);
    }

    [Fact]
    public async Task Handle_DecreaseWithinLimits_ReturnsSuccess()
    {
        var income = new Income
        {
            Id = 1, UserId = 1, Amount = 5000m,
            Date = new DateTime(2025, 6, 15), IncomeCategoryId = 1
        };
        _incomes.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);
        var category = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" };
        _incomeCategories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2000m);
        _expenses.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1500m);

        var command = new UpdateIncomeCommand(1, 1, 3000m, new DateTime(2025, 6, 15), 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DecreaseWouldExceedBudgets_ReturnsConflict()
    {
        var income = new Income
        {
            Id = 1, UserId = 1, Amount = 5000m,
            Date = new DateTime(2025, 6, 15), IncomeCategoryId = 1
        };
        _incomes.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);
        var category = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" };
        _incomeCategories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(4500m);
        _expenses.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var command = new UpdateIncomeCommand(1, 1, 1000m, new DateTime(2025, 6, 15), 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("budgets");
    }

    [Fact]
    public async Task Handle_DecreaseWouldExceedExpenses_ReturnsConflict()
    {
        var income = new Income
        {
            Id = 1, UserId = 1, Amount = 5000m,
            Date = new DateTime(2025, 6, 15), IncomeCategoryId = 1
        };
        _incomes.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);
        var category = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" };
        _incomeCategories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(500m);
        _expenses.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(4500m);

        var command = new UpdateIncomeCommand(1, 1, 1000m, new DateTime(2025, 6, 15), 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("expenses");
    }

    [Fact]
    public async Task Handle_NonExistentIncome_ReturnsNotFound()
    {
        _incomes.Setup(x => x.FindByIdAndUserAsync(99, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Income?)null);

        var command = new UpdateIncomeCommand(99, 1, 5000m, DateTime.Now, 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsNotFound()
    {
        var income = new Income
        {
            Id = 1, UserId = 1, Amount = 5000m,
            Date = new DateTime(2025, 6, 15), IncomeCategoryId = 1
        };
        _incomes.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);
        _incomeCategories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IncomeCategory?)null);

        var command = new UpdateIncomeCommand(1, 1, 5000m, new DateTime(2025, 6, 15), 99);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

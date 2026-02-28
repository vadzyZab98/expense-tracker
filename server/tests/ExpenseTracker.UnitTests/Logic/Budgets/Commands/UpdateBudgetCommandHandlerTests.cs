using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Budgets.UpdateBudget;
using ExpenseTracker.Logic.Common;

namespace ExpenseTracker.UnitTests.Logic.Budgets.Commands;

public sealed class UpdateBudgetCommandHandlerTests
{
    private readonly Mock<IMonthlyBudgetRepository> _budgets;
    private readonly Mock<ICategoryRepository> _categories;
    private readonly Mock<IIncomeRepository> _incomes;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UpdateBudgetCommandHandler _handler;

    public UpdateBudgetCommandHandlerTests()
    {
        _budgets = new Mock<IMonthlyBudgetRepository>();
        _categories = new Mock<ICategoryRepository>();
        _incomes = new Mock<IIncomeRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new UpdateBudgetCommandHandler(
            _budgets.Object, _categories.Object, _incomes.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var budget = new MonthlyBudget
        {
            Id = 1, UserId = 1, CategoryId = 1, Year = 2025, Month = 6, Amount = 500m,
            Category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" }
        };
        _budgets.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(budget);
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);

        var command = new UpdateBudgetCommand(1, 1, 1, 2025, 6, 800m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        budget.Amount.Should().Be(800m);
    }

    [Fact]
    public async Task Handle_WouldExceedIncome_ReturnsConflict()
    {
        var budget = new MonthlyBudget
        {
            Id = 1, UserId = 1, CategoryId = 1, Year = 2025, Month = 6, Amount = 500m,
            Category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" }
        };
        _budgets.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(budget);
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(800m);

        var command = new UpdateBudgetCommand(1, 1, 1, 2025, 6, 900m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
    }

    [Fact]
    public async Task Handle_ZeroIncome_ReturnsConflict()
    {
        var budget = new MonthlyBudget
        {
            Id = 1, UserId = 1, CategoryId = 1, Year = 2025, Month = 6, Amount = 500m,
            Category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" }
        };
        _budgets.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(budget);
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var command = new UpdateBudgetCommand(1, 1, 1, 2025, 7, 500m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("No income recorded");
    }

    [Fact]
    public async Task Handle_NonExistentBudget_ReturnsNotFound()
    {
        _budgets.Setup(x => x.FindByIdAndUserAsync(99, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MonthlyBudget?)null);

        var command = new UpdateBudgetCommand(99, 1, 1, 2025, 6, 500m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsNotFound()
    {
        var budget = new MonthlyBudget
        {
            Id = 1, UserId = 1, CategoryId = 1, Year = 2025, Month = 6, Amount = 500m,
            Category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" }
        };
        _budgets.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(budget);
        _categories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var command = new UpdateBudgetCommand(1, 1, 99, 2025, 6, 500m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

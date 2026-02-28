using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Budgets.CreateBudget;
using ExpenseTracker.Logic.Common;

namespace ExpenseTracker.UnitTests.Logic.Budgets.Commands;

public sealed class CreateBudgetCommandHandlerTests
{
    private readonly Mock<IMonthlyBudgetRepository> _budgets;
    private readonly Mock<ICategoryRepository> _categories;
    private readonly Mock<IIncomeRepository> _incomes;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateBudgetCommandHandler _handler;

    public CreateBudgetCommandHandlerTests()
    {
        _budgets = new Mock<IMonthlyBudgetRepository>();
        _categories = new Mock<ICategoryRepository>();
        _incomes = new Mock<IIncomeRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateBudgetCommandHandler(
            _budgets.Object, _categories.Object, _incomes.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithBudget()
    {
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var command = new CreateBudgetCommand(1, 1, 2025, 6, 500m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(500m);
        result.Value!.Year.Should().Be(2025);
        result.Value!.Month.Should().Be(6);
    }

    [Fact]
    public async Task Handle_ZeroIncome_ReturnsConflict()
    {
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var command = new CreateBudgetCommand(1, 1, 2025, 6, 500m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("No income recorded");
    }

    [Fact]
    public async Task Handle_WouldExceedIncome_ReturnsConflict()
    {
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(800m);

        var command = new CreateBudgetCommand(1, 1, 2025, 6, 300m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("exceed total income");
    }

    [Fact]
    public async Task Handle_ExactlyEqualsIncome_ReturnsSuccess()
    {
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(500m);

        var command = new CreateBudgetCommand(1, 1, 2025, 6, 500m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsNotFound()
    {
        _categories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var command = new CreateBudgetCommand(1, 99, 2025, 6, 500m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsBudget()
    {
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomes.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5000m);
        _budgets.Setup(x => x.GetTotalForMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var command = new CreateBudgetCommand(1, 1, 2025, 6, 500m);

        await _handler.Handle(command, CancellationToken.None);

        _budgets.Verify(x => x.AddAsync(
            It.Is<MonthlyBudget>(b =>
                b.UserId == 1 && b.CategoryId == 1 && b.Year == 2025 &&
                b.Month == 6 && b.Amount == 500m),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

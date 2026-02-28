using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Incomes.CreateIncome;

namespace ExpenseTracker.UnitTests.Logic.Incomes.Commands;

public sealed class CreateIncomeCommandHandlerTests
{
    private readonly Mock<IIncomeRepository> _incomes;
    private readonly Mock<IIncomeCategoryRepository> _incomeCategories;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateIncomeCommandHandler _handler;

    public CreateIncomeCommandHandlerTests()
    {
        _incomes = new Mock<IIncomeRepository>();
        _incomeCategories = new Mock<IIncomeCategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateIncomeCommandHandler(
            _incomes.Object, _incomeCategories.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithIncome()
    {
        var category = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" };
        _incomeCategories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var date = new DateTime(2025, 6, 15);
        var command = new CreateIncomeCommand(1, 5000m, date, 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(5000m);
        result.Value!.IncomeCategoryId.Should().Be(1);
        result.Value!.IncomeCategory.Name.Should().Be("Salary");
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsIncome()
    {
        var category = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" };
        _incomeCategories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var command = new CreateIncomeCommand(1, 5000m, new DateTime(2025, 6, 15), 1);

        await _handler.Handle(command, CancellationToken.None);

        _incomes.Verify(x => x.AddAsync(
            It.Is<Income>(i => i.UserId == 1 && i.Amount == 5000m && i.IncomeCategoryId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsNotFound()
    {
        _incomeCategories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IncomeCategory?)null);

        var command = new CreateIncomeCommand(1, 5000m, DateTime.Now, 99);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.IncomeCategories.CreateIncomeCategory;

namespace ExpenseTracker.UnitTests.Logic.IncomeCategories.Commands;

public sealed class CreateIncomeCategoryCommandHandlerTests
{
    private readonly Mock<IIncomeCategoryRepository> _incomeCategories;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateIncomeCategoryCommandHandler _handler;

    public CreateIncomeCategoryCommandHandlerTests()
    {
        _incomeCategories = new Mock<IIncomeCategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateIncomeCategoryCommandHandler(
            _incomeCategories.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithCategory()
    {
        var command = new CreateIncomeCategoryCommand("Salary", "#4CAF50");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Salary");
        result.Value!.Color.Should().Be("#4CAF50");
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsCategory()
    {
        var command = new CreateIncomeCategoryCommand("Salary", "#4CAF50");

        await _handler.Handle(command, CancellationToken.None);

        _incomeCategories.Verify(x => x.AddAsync(
            It.Is<IncomeCategory>(c => c.Name == "Salary" && c.Color == "#4CAF50"),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.IncomeCategories.DeleteIncomeCategory;

namespace ExpenseTracker.UnitTests.Logic.IncomeCategories.Commands;

public sealed class DeleteIncomeCategoryCommandHandlerTests
{
    private readonly Mock<IIncomeCategoryRepository> _incomeCategories;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly DeleteIncomeCategoryCommandHandler _handler;

    public DeleteIncomeCategoryCommandHandlerTests()
    {
        _incomeCategories = new Mock<IIncomeCategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteIncomeCategoryCommandHandler(
            _incomeCategories.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingCategoryWithoutIncomes_ReturnsSuccess()
    {
        var category = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" };
        _incomeCategories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomeCategories.Setup(x => x.HasIncomesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new DeleteIncomeCategoryCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _incomeCategories.Verify(x => x.Delete(category), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryWithIncomes_ReturnsConflict()
    {
        var category = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" };
        _incomeCategories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _incomeCategories.Setup(x => x.HasIncomesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new DeleteIncomeCategoryCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
    }

    [Fact]
    public async Task Handle_NonExistentCategory_ReturnsNotFound()
    {
        _incomeCategories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IncomeCategory?)null);

        var result = await _handler.Handle(new DeleteIncomeCategoryCommand(99), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

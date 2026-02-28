using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.IncomeCategories.UpdateIncomeCategory;

namespace ExpenseTracker.UnitTests.Logic.IncomeCategories.Commands;

public sealed class UpdateIncomeCategoryCommandHandlerTests
{
    private readonly Mock<IIncomeCategoryRepository> _incomeCategories;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UpdateIncomeCategoryCommandHandler _handler;

    public UpdateIncomeCategoryCommandHandlerTests()
    {
        _incomeCategories = new Mock<IIncomeCategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new UpdateIncomeCategoryCommandHandler(
            _incomeCategories.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingCategory_ReturnsSuccess()
    {
        var category = new IncomeCategory { Id = 1, Name = "Old", Color = "#000000" };
        _incomeCategories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var command = new UpdateIncomeCategoryCommand(1, "Updated", "#FFFFFF");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        category.Name.Should().Be("Updated");
        category.Color.Should().Be("#FFFFFF");
    }

    [Fact]
    public async Task Handle_NonExistentCategory_ReturnsNotFound()
    {
        _incomeCategories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IncomeCategory?)null);

        var command = new UpdateIncomeCategoryCommand(99, "Updated", "#FFFFFF");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

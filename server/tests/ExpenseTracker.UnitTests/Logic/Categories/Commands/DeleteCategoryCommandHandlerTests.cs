using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Categories.DeleteCategory;
using ExpenseTracker.Logic.Common;

namespace ExpenseTracker.UnitTests.Logic.Categories.Commands;

public sealed class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categories;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _categories = new Mock<ICategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteCategoryCommandHandler(_categories.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingCategoryWithNoExpenses_ReturnsSuccess()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _categories.Setup(x => x.HasExpensesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new DeleteCategoryCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _categories.Verify(x => x.Delete(category), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentCategory_ReturnsNotFound()
    {
        // Arrange
        _categories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var command = new DeleteCategoryCommand(99);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }

    [Fact]
    public async Task Handle_CategoryWithExpenses_ReturnsConflict()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _categories.Setup(x => x.HasExpensesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteCategoryCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.Conflict);
        result.Error!.Message.Should().Contain("linked expenses");
    }

    [Fact]
    public async Task Handle_CategoryWithExpenses_DoesNotDelete()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _categories.Setup(x => x.HasExpensesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteCategoryCommand(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _categories.Verify(x => x.Delete(It.IsAny<Category>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

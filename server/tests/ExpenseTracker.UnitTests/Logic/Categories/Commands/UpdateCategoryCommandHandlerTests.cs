using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Categories.UpdateCategory;
using ExpenseTracker.Logic.Common;

namespace ExpenseTracker.UnitTests.Logic.Categories.Commands;

public sealed class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categories;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _categories = new Mock<ICategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new UpdateCategoryCommandHandler(_categories.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingCategory_ReturnsSuccess()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Old", Color = "#000000" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var command = new UpdateCategoryCommand(1, "New Name", "#FF6B6B");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ExistingCategory_UpdatesFields()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Old", Color = "#000000" };
        _categories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var command = new UpdateCategoryCommand(1, "New Name", "#FF6B6B");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        category.Name.Should().Be("New Name");
        category.Color.Should().Be("#FF6B6B");
        _categories.Verify(x => x.Update(category), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentCategory_ReturnsNotFound()
    {
        // Arrange
        _categories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var command = new UpdateCategoryCommand(99, "Name", "#FF6B6B");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

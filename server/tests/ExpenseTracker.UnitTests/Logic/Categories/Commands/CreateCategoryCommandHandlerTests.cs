using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Categories.CreateCategory;

namespace ExpenseTracker.UnitTests.Logic.Categories.Commands;

public sealed class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categories;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _categories = new Mock<ICategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateCategoryCommandHandler(_categories.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithCategory()
    {
        // Arrange
        var command = new CreateCategoryCommand("Food", "#FF6B6B");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Food");
        result.Value!.Color.Should().Be("#FF6B6B");
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsCategory()
    {
        // Arrange
        var command = new CreateCategoryCommand("Food", "#FF6B6B");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _categories.Verify(x => x.AddAsync(
            It.Is<Category>(c => c.Name == "Food" && c.Color == "#FF6B6B"),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

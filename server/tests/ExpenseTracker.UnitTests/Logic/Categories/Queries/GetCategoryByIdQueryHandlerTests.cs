using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Categories.GetCategoryById;
using ExpenseTracker.Logic.Common;

namespace ExpenseTracker.UnitTests.Logic.Categories.Queries;

public sealed class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categories;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTests()
    {
        _categories = new Mock<ICategoryRepository>();
        _handler = new GetCategoryByIdQueryHandler(_categories.Object);
    }

    [Fact]
    public async Task Handle_CategoryExists_ReturnsSuccess()
    {
        // Arrange
        var category = new Category { Id = 7, Name = "Food", Color = "#FF6B6B" };
        _categories.Setup(x => x.FindByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _handler.Handle(new GetCategoryByIdQuery(7), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(7);
        result.Value.Name.Should().Be("Food");
        result.Value.Color.Should().Be("#FF6B6B");
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _categories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _handler.Handle(new GetCategoryByIdQuery(99), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

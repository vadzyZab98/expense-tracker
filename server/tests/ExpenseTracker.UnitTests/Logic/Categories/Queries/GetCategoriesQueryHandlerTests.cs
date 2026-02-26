using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Categories.GetCategories;
using ExpenseTracker.Logic.DTOs;

namespace ExpenseTracker.UnitTests.Logic.Categories.Queries;

public sealed class GetCategoriesQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categories;
    private readonly GetCategoriesQueryHandler _handler;

    public GetCategoriesQueryHandlerTests()
    {
        _categories = new Mock<ICategoryRepository>();
        _handler = new GetCategoriesQueryHandler(_categories.Object);
    }

    [Fact]
    public async Task Handle_CategoriesExist_ReturnsSortedByName()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Transport", Color = "#0000FF" },
            new() { Id = 2, Name = "Food", Color = "#FF6B6B" },
            new() { Id = 3, Name = "Rent", Color = "#00FF00" }
        };
        _categories.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var query = new GetCategoriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Food");
        result[1].Name.Should().Be("Rent");
        result[2].Name.Should().Be("Transport");
    }

    [Fact]
    public async Task Handle_NoCategories_ReturnsEmptyList()
    {
        // Arrange
        _categories.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new GetCategoriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CategoriesExist_MapsToCategoryResponse()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 5, Name = "Food", Color = "#FF6B6B" }
        };
        _categories.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var query = new GetCategoriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new CategoryResponse(5, "Food", "#FF6B6B"));
    }
}

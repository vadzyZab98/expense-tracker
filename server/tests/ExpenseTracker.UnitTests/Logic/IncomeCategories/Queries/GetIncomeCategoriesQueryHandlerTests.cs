using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.IncomeCategories.GetIncomeCategories;

namespace ExpenseTracker.UnitTests.Logic.IncomeCategories.Queries;

public sealed class GetIncomeCategoriesQueryHandlerTests
{
    private readonly Mock<IIncomeCategoryRepository> _incomeCategories;
    private readonly GetIncomeCategoriesQueryHandler _handler;

    public GetIncomeCategoriesQueryHandlerTests()
    {
        _incomeCategories = new Mock<IIncomeCategoryRepository>();
        _handler = new GetIncomeCategoriesQueryHandler(_incomeCategories.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllCategories()
    {
        var categories = new List<IncomeCategory>
        {
            new() { Id = 1, Name = "Salary", Color = "#4CAF50" },
            new() { Id = 2, Name = "Freelance", Color = "#2196F3" }
        };
        _incomeCategories.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var result = await _handler.Handle(new GetIncomeCategoriesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Salary");
        result[1].Name.Should().Be("Freelance");
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsEmptyCollection()
    {
        _incomeCategories.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IncomeCategory>());

        var result = await _handler.Handle(new GetIncomeCategoriesQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}

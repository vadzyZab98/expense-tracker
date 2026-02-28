using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Budgets.GetBudgets;

namespace ExpenseTracker.UnitTests.Logic.Budgets.Queries;

public sealed class GetBudgetsQueryHandlerTests
{
    private readonly Mock<IMonthlyBudgetRepository> _budgets;
    private readonly GetBudgetsQueryHandler _handler;

    public GetBudgetsQueryHandlerTests()
    {
        _budgets = new Mock<IMonthlyBudgetRepository>();
        _handler = new GetBudgetsQueryHandler(_budgets.Object);
    }

    [Fact]
    public async Task Handle_ReturnsUserBudgets()
    {
        var budgets = new List<MonthlyBudget>
        {
            new()
            {
                Id = 1, UserId = 1, CategoryId = 1, Year = 2025, Month = 6, Amount = 500m,
                Category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" }
            },
            new()
            {
                Id = 2, UserId = 1, CategoryId = 2, Year = 2025, Month = 6, Amount = 300m,
                Category = new Category { Id = 2, Name = "Transport", Color = "#4ECDC4" }
            }
        };
        _budgets.Setup(x => x.GetByUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(budgets);

        var result = await _handler.Handle(new GetBudgetsQuery(1), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsEmptyCollection()
    {
        _budgets.Setup(x => x.GetByUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MonthlyBudget>());

        var result = await _handler.Handle(new GetBudgetsQuery(1), CancellationToken.None);

        result.Should().BeEmpty();
    }
}

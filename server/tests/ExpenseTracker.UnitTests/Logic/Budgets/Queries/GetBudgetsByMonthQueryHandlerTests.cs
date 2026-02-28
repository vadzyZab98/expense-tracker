using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Budgets.GetBudgetsByMonth;

namespace ExpenseTracker.UnitTests.Logic.Budgets.Queries;

public sealed class GetBudgetsByMonthQueryHandlerTests
{
    private readonly Mock<IMonthlyBudgetRepository> _budgets;
    private readonly GetBudgetsByMonthQueryHandler _handler;

    public GetBudgetsByMonthQueryHandlerTests()
    {
        _budgets = new Mock<IMonthlyBudgetRepository>();
        _handler = new GetBudgetsByMonthQueryHandler(_budgets.Object);
    }

    [Fact]
    public async Task Handle_ReturnsFilteredBudgets()
    {
        var budgets = new List<MonthlyBudget>
        {
            new()
            {
                Id = 1, UserId = 1, CategoryId = 1, Year = 2025, Month = 6, Amount = 500m,
                Category = new Category { Id = 1, Name = "Food", Color = "#FF6B6B" }
            }
        };
        _budgets.Setup(x => x.GetByUserAndMonthAsync(1, 2025, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(budgets);

        var result = await _handler.Handle(
            new GetBudgetsByMonthQuery(1, 2025, 6), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Year.Should().Be(2025);
        result[0].Month.Should().Be(6);
    }

    [Fact]
    public async Task Handle_NoMatchingBudgets_ReturnsEmpty()
    {
        _budgets.Setup(x => x.GetByUserAndMonthAsync(1, 2025, 12, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MonthlyBudget>());

        var result = await _handler.Handle(
            new GetBudgetsByMonthQuery(1, 2025, 12), CancellationToken.None);

        result.Should().BeEmpty();
    }
}

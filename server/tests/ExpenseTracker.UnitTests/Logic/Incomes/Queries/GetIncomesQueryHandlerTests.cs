using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Incomes.GetIncomes;

namespace ExpenseTracker.UnitTests.Logic.Incomes.Queries;

public sealed class GetIncomesQueryHandlerTests
{
    private readonly Mock<IIncomeRepository> _incomes;
    private readonly GetIncomesQueryHandler _handler;

    public GetIncomesQueryHandlerTests()
    {
        _incomes = new Mock<IIncomeRepository>();
        _handler = new GetIncomesQueryHandler(_incomes.Object);
    }

    [Fact]
    public async Task Handle_ReturnsUserIncomes()
    {
        var incomes = new List<Income>
        {
            new()
            {
                Id = 1, UserId = 1, Amount = 5000m,
                Date = new DateTime(2025, 6, 15), IncomeCategoryId = 1,
                IncomeCategory = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" }
            },
            new()
            {
                Id = 2, UserId = 1, Amount = 1000m,
                Date = new DateTime(2025, 6, 20), IncomeCategoryId = 2,
                IncomeCategory = new IncomeCategory { Id = 2, Name = "Freelance", Color = "#2196F3" }
            }
        };
        _incomes.Setup(x => x.GetByUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(incomes);

        var result = await _handler.Handle(new GetIncomesQuery(1), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Amount.Should().Be(1000m); // Ordered by date desc — June 20 first
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsEmptyCollection()
    {
        _incomes.Setup(x => x.GetByUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Income>());

        var result = await _handler.Handle(new GetIncomesQuery(1), CancellationToken.None);

        result.Should().BeEmpty();
    }
}

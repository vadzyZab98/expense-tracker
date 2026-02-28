using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.Incomes.GetIncomeById;

namespace ExpenseTracker.UnitTests.Logic.Incomes.Queries;

public sealed class GetIncomeByIdQueryHandlerTests
{
    private readonly Mock<IIncomeRepository> _incomes;
    private readonly GetIncomeByIdQueryHandler _handler;

    public GetIncomeByIdQueryHandlerTests()
    {
        _incomes = new Mock<IIncomeRepository>();
        _handler = new GetIncomeByIdQueryHandler(_incomes.Object);
    }

    [Fact]
    public async Task Handle_ExistingIncome_ReturnsSuccess()
    {
        var income = new Income
        {
            Id = 1, UserId = 1, Amount = 5000m,
            Date = new DateTime(2025, 6, 15), IncomeCategoryId = 1,
            IncomeCategory = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" }
        };
        _incomes.Setup(x => x.FindByIdAndUserAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(income);

        var result = await _handler.Handle(
            new GetIncomeByIdQuery(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(5000m);
    }

    [Fact]
    public async Task Handle_NonExistentIncome_ReturnsNotFound()
    {
        _incomes.Setup(x => x.FindByIdAndUserAsync(99, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Income?)null);

        var result = await _handler.Handle(
            new GetIncomeByIdQuery(99, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

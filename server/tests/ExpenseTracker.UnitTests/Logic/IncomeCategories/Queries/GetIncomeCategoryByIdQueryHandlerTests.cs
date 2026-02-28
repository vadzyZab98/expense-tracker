using Moq;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.IncomeCategories.GetIncomeCategoryById;

namespace ExpenseTracker.UnitTests.Logic.IncomeCategories.Queries;

public sealed class GetIncomeCategoryByIdQueryHandlerTests
{
    private readonly Mock<IIncomeCategoryRepository> _incomeCategories;
    private readonly GetIncomeCategoryByIdQueryHandler _handler;

    public GetIncomeCategoryByIdQueryHandlerTests()
    {
        _incomeCategories = new Mock<IIncomeCategoryRepository>();
        _handler = new GetIncomeCategoryByIdQueryHandler(_incomeCategories.Object);
    }

    [Fact]
    public async Task Handle_ExistingCategory_ReturnsSuccess()
    {
        var category = new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" };
        _incomeCategories.Setup(x => x.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _handler.Handle(
            new GetIncomeCategoryByIdQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Salary");
    }

    [Fact]
    public async Task Handle_NonExistentCategory_ReturnsNotFound()
    {
        _incomeCategories.Setup(x => x.FindByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IncomeCategory?)null);

        var result = await _handler.Handle(
            new GetIncomeCategoryByIdQuery(99), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
    }
}

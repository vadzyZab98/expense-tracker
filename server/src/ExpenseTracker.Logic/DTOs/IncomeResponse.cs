namespace ExpenseTracker.Logic.DTOs;

public sealed record IncomeResponse(
    int Id,
    decimal Amount,
    DateTime Date,
    int IncomeCategoryId,
    IncomeCategoryResponse IncomeCategory);

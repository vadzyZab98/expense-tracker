namespace ExpenseTracker.Logic.DTOs;

public sealed record ExpenseResponse(
    int Id,
    decimal Amount,
    string Description,
    DateTime Date,
    int CategoryId,
    CategoryResponse Category);

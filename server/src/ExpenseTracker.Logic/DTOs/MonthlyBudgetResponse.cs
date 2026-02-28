namespace ExpenseTracker.Logic.DTOs;

public sealed record MonthlyBudgetResponse(
    int Id,
    int CategoryId,
    CategoryResponse Category,
    int Year,
    int Month,
    decimal Amount);

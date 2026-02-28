namespace ExpenseTracker.Core.Entities;

public class MonthlyBudget
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Amount { get; set; }
}

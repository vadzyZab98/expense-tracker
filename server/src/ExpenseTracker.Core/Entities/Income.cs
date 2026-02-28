namespace ExpenseTracker.Core.Entities;

public class Income
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int IncomeCategoryId { get; set; }
    public IncomeCategory IncomeCategory { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}

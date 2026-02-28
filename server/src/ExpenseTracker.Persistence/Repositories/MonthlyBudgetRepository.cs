using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence.Repositories;

public sealed class MonthlyBudgetRepository : IMonthlyBudgetRepository
{
    private readonly AppDbContext _db;

    public MonthlyBudgetRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<MonthlyBudget>> GetByUserAsync(
        int userId, CancellationToken ct = default)
        => await _db.MonthlyBudgets
            .Where(b => b.UserId == userId)
            .Include(b => b.Category)
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<MonthlyBudget>> GetByUserAndMonthAsync(
        int userId, int year, int month, CancellationToken ct = default)
        => await _db.MonthlyBudgets
            .Where(b => b.UserId == userId && b.Year == year && b.Month == month)
            .Include(b => b.Category)
            .ToListAsync(ct);

    public Task<MonthlyBudget?> FindByIdAndUserAsync(
        int id, int userId, CancellationToken ct = default)
        => _db.MonthlyBudgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId, ct);

    public async Task AddAsync(MonthlyBudget budget, CancellationToken ct = default)
        => await _db.MonthlyBudgets.AddAsync(budget, ct);

    public void Update(MonthlyBudget budget)
        => _db.MonthlyBudgets.Update(budget);

    public void Delete(MonthlyBudget budget)
        => _db.MonthlyBudgets.Remove(budget);

    public async Task<decimal> GetTotalForMonthAsync(
        int userId, int year, int month, CancellationToken ct = default)
    {
        var total = await _db.MonthlyBudgets
            .Where(b => b.UserId == userId && b.Year == year && b.Month == month)
            .SumAsync(b => (double)b.Amount, ct);
        return (decimal)total;
    }
}

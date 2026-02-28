using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence.Repositories;

public sealed class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _db;

    public ExpenseRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Expense>> GetByUserAsync(int userId, CancellationToken ct = default)
        => await _db.Expenses
            .Where(e => e.UserId == userId)
            .Include(e => e.Category)
            .OrderByDescending(e => e.Date)
            .ToListAsync(ct);

    public Task<Expense?> FindByIdAndUserAsync(int id, int userId, CancellationToken ct = default)
        => _db.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, ct);

    public async Task AddAsync(Expense expense, CancellationToken ct = default)
        => await _db.Expenses.AddAsync(expense, ct);

    public void Update(Expense expense)
        => _db.Expenses.Update(expense);

    public void Delete(Expense expense)
        => _db.Expenses.Remove(expense);

    public async Task<decimal> GetTotalForMonthAsync(
        int userId, int year, int month, CancellationToken ct = default)
    {
        var total = await _db.Expenses
            .Where(e => e.UserId == userId && e.Date.Year == year && e.Date.Month == month)
            .SumAsync(e => (double)e.Amount, ct);
        return (decimal)total;
    }
}

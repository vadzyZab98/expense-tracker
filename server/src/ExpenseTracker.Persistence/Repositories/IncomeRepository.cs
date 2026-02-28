using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence.Repositories;

public sealed class IncomeRepository : IIncomeRepository
{
    private readonly AppDbContext _db;

    public IncomeRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Income>> GetByUserAsync(int userId, CancellationToken ct = default)
        => await _db.Incomes
            .Where(i => i.UserId == userId)
            .Include(i => i.IncomeCategory)
            .OrderByDescending(i => i.Date)
            .ToListAsync(ct);

    public Task<Income?> FindByIdAndUserAsync(int id, int userId, CancellationToken ct = default)
        => _db.Incomes
            .Include(i => i.IncomeCategory)
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId, ct);

    public async Task AddAsync(Income income, CancellationToken ct = default)
        => await _db.Incomes.AddAsync(income, ct);

    public void Update(Income income)
        => _db.Incomes.Update(income);

    public void Delete(Income income)
        => _db.Incomes.Remove(income);

    public async Task<decimal> GetTotalForMonthAsync(
        int userId, int year, int month, CancellationToken ct = default)
    {
        var total = await _db.Incomes
            .Where(i => i.UserId == userId && i.Date.Year == year && i.Date.Month == month)
            .SumAsync(i => (double)i.Amount, ct);
        return (decimal)total;
    }
}

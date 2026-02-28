using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence.Repositories;

public sealed class IncomeCategoryRepository : IIncomeCategoryRepository
{
    private readonly AppDbContext _db;

    public IncomeCategoryRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<IncomeCategory>> GetAllAsync(CancellationToken ct = default)
        => await _db.IncomeCategories
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

    public Task<IncomeCategory?> FindByIdAsync(int id, CancellationToken ct = default)
        => _db.IncomeCategories.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(IncomeCategory category, CancellationToken ct = default)
        => await _db.IncomeCategories.AddAsync(category, ct);

    public void Update(IncomeCategory category)
        => _db.IncomeCategories.Update(category);

    public void Delete(IncomeCategory category)
        => _db.IncomeCategories.Remove(category);

    public Task<bool> HasIncomesAsync(int categoryId, CancellationToken ct = default)
        => _db.Incomes.AnyAsync(i => i.IncomeCategoryId == categoryId, ct);
}

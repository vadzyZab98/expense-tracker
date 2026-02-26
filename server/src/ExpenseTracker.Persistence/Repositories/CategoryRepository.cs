using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default)
        => await _db.Categories.AsNoTracking().ToListAsync(ct);

    public Task<Category?> FindByIdAsync(int id, CancellationToken ct = default)
        => _db.Categories.FindAsync([id], ct).AsTask();

    public async Task AddAsync(Category category, CancellationToken ct = default)
        => await _db.Categories.AddAsync(category, ct);

    public void Update(Category category)
        => _db.Categories.Update(category);

    public void Delete(Category category)
        => _db.Categories.Remove(category);

    public Task<bool> HasExpensesAsync(int categoryId, CancellationToken ct = default)
        => _db.Expenses.AnyAsync(e => e.CategoryId == categoryId, ct);
}

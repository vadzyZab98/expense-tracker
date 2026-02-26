using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default);
    Task<Category?> FindByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
    void Update(Category category);
    void Delete(Category category);
    Task<bool> HasExpensesAsync(int categoryId, CancellationToken ct = default);
}

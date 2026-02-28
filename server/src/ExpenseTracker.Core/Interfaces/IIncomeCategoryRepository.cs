using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Interfaces;

public interface IIncomeCategoryRepository
{
    Task<IReadOnlyList<IncomeCategory>> GetAllAsync(CancellationToken ct = default);
    Task<IncomeCategory?> FindByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(IncomeCategory category, CancellationToken ct = default);
    void Update(IncomeCategory category);
    void Delete(IncomeCategory category);
    Task<bool> HasIncomesAsync(int categoryId, CancellationToken ct = default);
}

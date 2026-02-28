using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Interfaces;

public interface IMonthlyBudgetRepository
{
    Task<IReadOnlyList<MonthlyBudget>> GetByUserAsync(int userId, CancellationToken ct = default);
    Task<IReadOnlyList<MonthlyBudget>> GetByUserAndMonthAsync(int userId, int year, int month, CancellationToken ct = default);
    Task<MonthlyBudget?> FindByIdAndUserAsync(int id, int userId, CancellationToken ct = default);
    Task AddAsync(MonthlyBudget budget, CancellationToken ct = default);
    void Update(MonthlyBudget budget);
    void Delete(MonthlyBudget budget);
    Task<decimal> GetTotalForMonthAsync(int userId, int year, int month, CancellationToken ct = default);
}

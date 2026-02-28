using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Interfaces;

public interface IIncomeRepository
{
    Task<IReadOnlyList<Income>> GetByUserAsync(int userId, CancellationToken ct = default);
    Task<Income?> FindByIdAndUserAsync(int id, int userId, CancellationToken ct = default);
    Task AddAsync(Income income, CancellationToken ct = default);
    void Update(Income income);
    void Delete(Income income);
    Task<decimal> GetTotalForMonthAsync(int userId, int year, int month, CancellationToken ct = default);
}

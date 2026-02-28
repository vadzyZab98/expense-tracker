using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Interfaces;

public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> GetByUserAsync(int userId, CancellationToken ct = default);
    Task<Expense?> FindByIdAndUserAsync(int id, int userId, CancellationToken ct = default);
    Task AddAsync(Expense expense, CancellationToken ct = default);
    void Update(Expense expense);
    void Delete(Expense expense);
    Task<decimal> GetTotalForMonthAsync(int userId, int year, int month, CancellationToken ct = default);
}

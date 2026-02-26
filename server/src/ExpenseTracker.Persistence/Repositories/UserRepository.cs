using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.SingleOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> FindByIdAsync(int id, CancellationToken ct = default)
        => _db.Users.FindAsync([id], ct).AsTask();

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
        => await _db.Users.AsNoTracking().OrderBy(u => u.Id).ToListAsync(ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _db.Users.AddAsync(user, ct);
}

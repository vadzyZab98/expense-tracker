using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Logic.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}

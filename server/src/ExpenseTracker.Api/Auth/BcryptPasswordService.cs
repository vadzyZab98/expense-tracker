using ExpenseTracker.Logic.Interfaces;

namespace ExpenseTracker.Api.Auth;

public sealed class BcryptPasswordService : IPasswordService
{
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}

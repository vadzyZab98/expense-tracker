using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using ExpenseTracker.Logic.Interfaces;
using MediatR;

namespace ExpenseTracker.Logic.Auth.Login;

public sealed class LoginQueryHandler : IRequestHandler<LoginQuery, Result<TokenResponse>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordService _passwords;
    private readonly ITokenService _tokens;

    public LoginQueryHandler(
        IUserRepository users,
        IPasswordService passwords,
        ITokenService tokens)
    {
        _users = users;
        _passwords = passwords;
        _tokens = tokens;
    }

    public async Task<Result<TokenResponse>> Handle(LoginQuery request, CancellationToken ct)
    {
        var user = await _users.FindByEmailAsync(request.Email, ct);

        if (user is null || !_passwords.Verify(request.Password, user.PasswordHash))
            return Result<TokenResponse>.Failure(DomainError.Unauthorized("Invalid email or password."));

        return Result<TokenResponse>.Success(new TokenResponse(_tokens.GenerateToken(user)));
    }
}

using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using ExpenseTracker.Logic.Interfaces;
using MediatR;

namespace ExpenseTracker.Logic.Auth.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<TokenResponse>>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwords;
    private readonly ITokenService _tokens;

    public RegisterCommandHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        IPasswordService passwords,
        ITokenService tokens)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _passwords = passwords;
        _tokens = tokens;
    }

    public async Task<Result<TokenResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existing = await _users.FindByEmailAsync(request.Email, ct);
        if (existing is not null)
            return Result<TokenResponse>.Failure(DomainError.Conflict("Email already in use."));

        var isFirstUser = await _users.IsEmptyAsync(ct);

        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwords.Hash(request.Password),
            Role = isFirstUser ? "Admin" : "User"
        };

        await _users.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<TokenResponse>.Success(new TokenResponse(_tokens.GenerateToken(user)));
    }
}

using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Auth.Login;

public sealed record LoginQuery(string Email, string Password) : IRequest<Result<TokenResponse>>;

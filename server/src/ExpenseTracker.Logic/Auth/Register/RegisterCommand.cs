using ExpenseTracker.Logic.Common;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Auth.Register;

public sealed record RegisterCommand(string Email, string Password) : IRequest<Result<TokenResponse>>;

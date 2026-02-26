using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Users.AssignRole;

public sealed record AssignRoleCommand(int UserId, string Role) : IRequest<Result>;

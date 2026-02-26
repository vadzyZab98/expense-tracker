using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Users.GetUsers;

public sealed record GetUsersQuery() : IRequest<IReadOnlyList<UserResponse>>;

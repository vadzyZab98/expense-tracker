using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.DTOs;
using MediatR;

namespace ExpenseTracker.Logic.Users.GetUsers;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserResponse>>
{
    private readonly IUserRepository _users;

    public GetUsersQueryHandler(IUserRepository users) => _users = users;

    public async Task<IReadOnlyList<UserResponse>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await _users.GetAllAsync(ct);
        return users.Select(u => new UserResponse(u.Id, u.Email, u.Role)).ToList();
    }
}

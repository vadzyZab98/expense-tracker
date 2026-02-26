using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Logic.Common;
using MediatR;

namespace ExpenseTracker.Logic.Users.AssignRole;

public sealed class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Result>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleCommandHandler(IUserRepository users, IUnitOfWork unitOfWork)
    {
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken ct)
    {
        var user = await _users.FindByIdAsync(request.UserId, ct);
        if (user is null)
            return Result.Failure(DomainError.NotFound("User", request.UserId));

        if (user.Role == "SuperAdmin")
            return Result.Failure(DomainError.Conflict("Cannot change the role of a SuperAdmin."));

        user.Role = request.Role;
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

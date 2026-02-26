using FluentValidation;

namespace ExpenseTracker.Logic.Users.AssignRole;

public sealed class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    private static readonly string[] AllowedRoles = ["User", "Admin"];

    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => AllowedRoles.Contains(r))
            .WithMessage("Role must be 'User' or 'Admin'.");
    }
}

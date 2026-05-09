using FluentValidation;

namespace CleanArchitecture.Application.Features.Roles.Commands.AssignRole;

public sealed class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("RoleId is required.");
    }
}

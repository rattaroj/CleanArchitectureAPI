using FluentValidation;

namespace CleanArchitecture.Application.Features.Roles.Commands.RemoveRole;

public sealed class RemoveRoleCommandValidator : AbstractValidator<RemoveRoleCommand>
{
    public RemoveRoleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("RoleId is required.");
    }
}

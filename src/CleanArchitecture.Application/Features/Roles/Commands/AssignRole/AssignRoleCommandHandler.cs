using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Commands.AssignRole;

public sealed class AssignRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AssignRoleCommand, Result>
{
    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                Error.NotFound("Users.UserNotFound", "The user with the specified ID was not found."));
        }

        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
        {
            return Result.Failure(
                Error.NotFound("Roles.RoleNotFound", "The role with the specified ID was not found."));
        }

        var alreadyAssigned = await roleRepository.IsRoleAssignedAsync(
            request.UserId, request.RoleId, cancellationToken);

        if (alreadyAssigned)
        {
            return Result.Failure(
                Error.Conflict("Roles.AlreadyAssigned", "The user already has this role."));
        }

        var userRole = UserRole.Create(request.UserId, request.RoleId);
        await roleRepository.AssignRoleAsync(userRole, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

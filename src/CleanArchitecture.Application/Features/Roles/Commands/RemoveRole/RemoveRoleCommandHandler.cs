using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Common.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Commands.RemoveRole;

public sealed class RemoveRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveRoleCommand, Result>
{
    public async Task<Result> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                Error.NotFound("Users.UserNotFound", "The user with the specified ID was not found."));
        }

        var assigned = await roleRepository.IsRoleAssignedAsync(
            request.UserId, request.RoleId, cancellationToken);

        if (!assigned)
        {
            return Result.Failure(
                Error.NotFound("Roles.NotAssigned", "The user does not have this role."));
        }

        await roleRepository.RemoveRoleAsync(request.UserId, request.RoleId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

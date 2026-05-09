using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Common.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetUserRoles;

public sealed class GetUserRolesQueryHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository)
    : IRequestHandler<GetUserRolesQuery, Result<IReadOnlyList<UserRoleDto>>>
{
    public async Task<Result<IReadOnlyList<UserRoleDto>>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<IReadOnlyList<UserRoleDto>>(
                Error.NotFound("Users.UserNotFound", "The user with the specified ID was not found."));
        }

        var userRoles = await roleRepository.GetUserRolesAsync(request.UserId, cancellationToken);

        var dtos = userRoles
            .Select(ur => new UserRoleDto(ur.RoleId, ur.Role.Name, ur.Role.Description, ur.AssignedAt))
            .ToList();

        return Result.Success<IReadOnlyList<UserRoleDto>>(dtos);
    }
}

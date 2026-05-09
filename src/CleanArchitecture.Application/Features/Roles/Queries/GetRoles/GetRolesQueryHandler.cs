using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Common.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetRoles;

public sealed class GetRolesQueryHandler(IRoleRepository roleRepository)
    : IRequestHandler<GetRolesQuery, Result<IReadOnlyList<RoleDto>>>
{
    public async Task<Result<IReadOnlyList<RoleDto>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleRepository.GetAllAsync(cancellationToken);
        var dtos = roles.Select(r => new RoleDto(r.Id, r.Name, r.Description)).ToList();
        return Result.Success<IReadOnlyList<RoleDto>>(dtos);
    }
}

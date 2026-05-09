using CleanArchitecture.Application.Common.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetUserRoles;

public sealed record GetUserRolesQuery(Guid UserId) : IRequest<Result<IReadOnlyList<UserRoleDto>>>;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetUserRoles;

public sealed record UserRoleDto(Guid RoleId, string Name, string Description, DateTime AssignedAt);

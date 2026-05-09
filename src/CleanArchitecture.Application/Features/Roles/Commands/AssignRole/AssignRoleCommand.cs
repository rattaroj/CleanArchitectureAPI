using CleanArchitecture.Application.Common.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Commands.AssignRole;

public sealed record AssignRoleCommand(Guid UserId, Guid RoleId) : IRequest<Result>;

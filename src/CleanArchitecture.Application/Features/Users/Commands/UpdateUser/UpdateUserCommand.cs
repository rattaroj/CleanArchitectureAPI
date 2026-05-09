using CleanArchitecture.Application.Common.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(Guid Id, string Name, string Email)
    : IRequest<Result<UpdateUserResponse>>;

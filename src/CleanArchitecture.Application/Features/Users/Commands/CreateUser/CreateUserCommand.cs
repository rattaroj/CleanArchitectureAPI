using CleanArchitecture.Application.Common.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(string Name, string Email) : IRequest<Result<CreateUserResponse>>;
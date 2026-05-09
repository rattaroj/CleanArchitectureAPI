namespace CleanArchitecture.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserResponse(Guid Id, string Name, string Email);

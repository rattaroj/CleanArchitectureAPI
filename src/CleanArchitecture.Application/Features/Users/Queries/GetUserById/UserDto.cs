namespace CleanArchitecture.Application.Features.Users.Queries.GetUserById;

public sealed record UserDto(
    Guid Id,
    string Name,
    string Email,
    bool IsActive);

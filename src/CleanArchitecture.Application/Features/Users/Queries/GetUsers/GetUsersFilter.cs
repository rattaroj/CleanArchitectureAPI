namespace CleanArchitecture.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersFilter(
    string? NameContains = null,
    string? EmailEquals = null,
    bool? IsActive = null);

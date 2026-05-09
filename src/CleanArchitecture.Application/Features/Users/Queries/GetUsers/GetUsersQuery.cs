using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Features.Users.Queries.GetUserById;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    UserSortColumn SortBy = UserSortColumn.Name,
    SortDirection Direction = SortDirection.Asc,
    string? NameContains = null,
    string? EmailEquals = null,
    bool? IsActive = null)
    : IRequest<Result<PagedResult<UserDto>>>;

using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Features.Users.Queries.GetUserById;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDto>>>
{
    public async Task<Result<PagedResult<UserDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = new PaginationRequest(request.PageNumber, request.PageSize);
        var filter = new GetUsersFilter(request.NameContains, request.EmailEquals, request.IsActive);
        var sort = new SortRequest<UserSortColumn>(request.SortBy, request.Direction);

        var users = await userRepository.GetPagedAsync(pagination, filter, sort, cancellationToken);

        var response = users.Map(user => new UserDto(
            user.Id,
            user.Name,
            user.Email.Value,
            user.IsActive));

        return Result.Success(response);
    }
}

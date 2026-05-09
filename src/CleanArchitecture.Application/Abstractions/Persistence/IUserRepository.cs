using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Features.Users.Queries.GetUsers;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<User>> GetPagedAsync(
        PaginationRequest pagination,
        GetUsersFilter filter,
        SortRequest<UserSortColumn> sort,
        CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task DeleteAsync(User user, CancellationToken cancellationToken);
}

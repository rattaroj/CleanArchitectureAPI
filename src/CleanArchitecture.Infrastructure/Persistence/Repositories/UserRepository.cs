using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Features.Users.Queries.GetUsers;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedResult<User>> GetPagedAsync(
        PaginationRequest pagination,
        GetUsersFilter filter,
        SortRequest<UserSortColumn> sort,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pagination);
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(sort);

        var query = dbContext.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.NameContains))
        {
            var name = filter.NameContains.Trim();
            query = query.Where(u => u.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(filter.EmailEquals))
        {
            var email = Email.Create(filter.EmailEquals.Trim().ToLowerInvariant());
            query = query.Where(u => u.Email == email);
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == filter.IsActive.Value);
        }

        query = (sort.SortBy, sort.Direction) switch
        {
            (UserSortColumn.Name,     SortDirection.Asc)  => query.OrderBy(u => u.Name).ThenBy(u => u.Id),
            (UserSortColumn.Name,     SortDirection.Desc) => query.OrderByDescending(u => u.Name).ThenBy(u => u.Id),
            (UserSortColumn.Email,    SortDirection.Asc)  => query.OrderBy(u => u.Email).ThenBy(u => u.Id),
            (UserSortColumn.Email,    SortDirection.Desc) => query.OrderByDescending(u => u.Email).ThenBy(u => u.Id),
            (UserSortColumn.IsActive, SortDirection.Asc)  => query.OrderBy(u => u.IsActive).ThenBy(u => u.Name).ThenBy(u => u.Id),
            (UserSortColumn.IsActive, SortDirection.Desc) => query.OrderByDescending(u => u.IsActive).ThenBy(u => u.Name).ThenBy(u => u.Id),
            _ => query.OrderBy(u => u.Name).ThenBy(u => u.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<User>.Create(
            items,
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = Email.Create(email);

        return await dbContext.Users
            .AnyAsync(x => x.Email == normalizedEmail, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        dbContext.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        dbContext.Users.Remove(user);
        return Task.CompletedTask;
    }
}

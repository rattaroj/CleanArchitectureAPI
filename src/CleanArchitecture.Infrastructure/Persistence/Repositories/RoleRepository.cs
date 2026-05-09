using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(ApplicationDbContext dbContext) : IRoleRepository
{
    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await dbContext.Roles
            .AnyAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.UserRoles
            .AsNoTracking()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsRoleAssignedAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        return await dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    public Task AssignRoleAsync(UserRole userRole, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userRole);
        dbContext.UserRoles.Add(userRole);
        return Task.CompletedTask;
    }

    public async Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        var userRole = await dbContext.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

        if (userRole is not null)
            dbContext.UserRoles.Remove(userRole);
    }
}

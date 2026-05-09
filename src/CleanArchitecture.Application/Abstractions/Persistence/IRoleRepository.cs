using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Abstractions.Persistence;

public interface IRoleRepository
{
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken);
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> IsRoleAssignedAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task AssignRoleAsync(UserRole userRole, CancellationToken cancellationToken);
    Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
}

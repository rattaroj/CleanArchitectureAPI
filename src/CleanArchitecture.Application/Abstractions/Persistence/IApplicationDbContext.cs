using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

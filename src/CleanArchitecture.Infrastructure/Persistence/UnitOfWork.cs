using CleanArchitecture.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new PersistenceException("Concurrency conflict occurred.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new PersistenceException("Database update failed.", ex);
        }
    }
}
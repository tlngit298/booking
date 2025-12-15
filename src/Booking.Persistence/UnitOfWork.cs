using Booking.Application.Common;
using Booking.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Booking.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public IEnumerable<IDomainEvent> GetDomainEvents()
    {
        // Find all tracked aggregate roots in the change tracker
        var aggregateRoots = _dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();

        // Collect all events from all aggregates
        return aggregateRoots
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList();
    }

    public void ClearDomainEvents()
    {
        // Find all tracked aggregate roots
        var aggregateRoots = _dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();

        // Clear events from each aggregate
        foreach (var aggregate in aggregateRoots)
        {
            aggregate.ClearDomainEvents();
        }
    }
}

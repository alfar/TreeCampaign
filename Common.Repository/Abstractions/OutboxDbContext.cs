using Common.Domain.Abstractions;
using Common.Repository.Configurations;
using Common.Repository.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Repository.Abstractions;

public abstract class OutboxDbContext(DbContextOptions options) : DbContext(options)
{
    protected DbSet<StoredDomainEvent> StoredDomainEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddStoredDomainEventsWithoutMigrations();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries<IHasDomainEvents>();

        var events = entities.SelectMany(e => e.Entity.NewEvents).ToList();

        if (events.Count > 0)
        {
            StoredDomainEvents.AddRange(
                events.Select(e => new StoredDomainEvent
                {
                    AggregateId = e.AggregateId,
                    OccurredAtUtc = e.OccurredOn.UtcDateTime,
                    Type = e.GetType().FullName!,
                    Data = System.Text.Json.JsonSerializer.Serialize(e, e.GetType()),
                })
            );
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entry in entities)
        {
            entry.Entity.ClearEvents();
        }

        return result;
    }
}
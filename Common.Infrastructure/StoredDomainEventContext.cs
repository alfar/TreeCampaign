using Common.Infrastructure.Configurations;
using Common.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class StoredDomainEventContext : DbContext
{
    public DbSet<StoredDomainEvent> StoredDomainEvents { get; set; }

    public StoredDomainEventContext(DbContextOptions<StoredDomainEventContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddStoredDomainEvents();
    }

    public IQueryable<StoredDomainEvent> UnprocessedEvents => StoredDomainEvents.Where(e => e.ProcessedAtUtc == null);
}

public class StoredDomainEventContextFactory : IDesignTimeDbContextFactory<StoredDomainEventContext>
{
    public StoredDomainEventContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StoredDomainEventContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TreeCampaign;Trusted_Connection=True;");

        return new StoredDomainEventContext(optionsBuilder.Options);
    }
}

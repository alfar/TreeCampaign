using System.Threading.Channels;
using Access.Domain.ScoutGroups;
using Access.Domain.ScoutGroups.ValueObjects;
using Access.Domain.Users;
using Access.Domain.Users.ValueObjects;
using Access.Infrastructure.Configuration;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.BackgroundWorkers.Signals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Access.Infrastructure;

public class AccessContext(DbContextOptions<AccessContext> options, ChannelWriter<EventDispatchSignal> eventDispatcher)
    : OutboxDbContext(options, eventDispatcher),
        IAccessUnitOfWork,
        IRepository<ScoutGroup, ScoutGroupId>,
        IRepository<User, UserId>
{
    public DbSet<ScoutGroup> ScoutGroups { get; set; }
    public DbSet<User> Users { get; set; }

    public void Add(ScoutGroup aggregate) => ScoutGroups.Add(aggregate);

    public void Delete(ScoutGroup aggregate) => ScoutGroups.Remove(aggregate);

    async Task<ScoutGroup?> IRepository<ScoutGroup, ScoutGroupId>.TryFindAsync(ScoutGroupId key, CancellationToken cancellationToken) =>
        await ScoutGroups.FirstOrDefaultAsync(g => g.Id == key, cancellationToken);

    public void Add(User aggregate) => Users.Add(aggregate);

    public void Delete(User aggregate) => Users.Remove(aggregate);

    async Task<User?> IRepository<User, UserId>.TryFindAsync(UserId key, CancellationToken cancellationToken) =>
        await Users.FirstOrDefaultAsync(u => u.Id == key, cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddScoutGroups().AddUsers();
    }

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() =>
        (IRepository<TAggregate, TKey>)this;
}

public class AccessContextFactory : IDesignTimeDbContextFactory<AccessContext>
{
    public AccessContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AccessContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TreeCampaign;Trusted_Connection=True;");

        return new AccessContext(optionsBuilder.Options, null!);
    }
}

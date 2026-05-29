using Common.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Repository.Events;

namespace TreeCampaign.Repository;

public class TreeCampaignContext(DbContextOptions<TreeCampaignContext> options)
    : DbContext(options),
        IUnitOfWork,
        IRepository<Campaign, CampaignId>,
        IRepository<UnassignedStop, StopId>,
        IRepository<AssignedStop, StopId>,
        IRepository<ReopenableStop, StopId>,
        IRepository<CollectedStop, StopId>,
        IRepository<UnresolvedStop, StopId>,
        IRepository<Team, TeamId>
{
    public DbSet<Campaign> CollectionCampaigns { get; set; }
    internal DbSet<StopBase> Stops { get; set; }
    public DbSet<UnassignedStop> UnassignedStops { get; set; }
    public DbSet<AssignedStop> AssignedStops { get; set; }
    public DbSet<ReopenableStop> ReopenableStops { get; set; }
    public DbSet<CollectedStop> CollectedStops { get; set; }
    public DbSet<UnresolvedStop> UnresolvedStops { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<StoredDomainEvent> StoredDomainEvents { get; set; }

    public void Add(Campaign aggregate) => CollectionCampaigns.Add(aggregate);

    public void Delete(Campaign aggregate) => CollectionCampaigns.Remove(aggregate);

    async Task<Campaign?> IRepository<Campaign, CampaignId>.TryFindAsync(CampaignId key) =>
        await CollectionCampaigns.FirstOrDefaultAsync(c => c.Id == key);

    public void Add(UnassignedStop aggregate) => UnassignedStops.Add(aggregate);

    public void Delete(UnassignedStop aggregate) => UnassignedStops.Remove(aggregate);

    async Task<UnassignedStop?> IRepository<UnassignedStop, StopId>.TryFindAsync(StopId key) =>
        await UnassignedStops.FirstOrDefaultAsync(s => s.Id == key);

    public void Add(AssignedStop aggregate) => AssignedStops.Add(aggregate);

    public void Delete(AssignedStop aggregate) => AssignedStops.Remove(aggregate);

    public void Add(ReopenableStop aggregate) => ReopenableStops.Add(aggregate);

    public void Delete(ReopenableStop aggregate) => ReopenableStops.Remove(aggregate);

    async Task<ReopenableStop?> IRepository<ReopenableStop, StopId>.TryFindAsync(StopId key) =>
        await ReopenableStops.FirstOrDefaultAsync(s => s.Id == key);

    async Task<AssignedStop?> IRepository<AssignedStop, StopId>.TryFindAsync(StopId key) =>
        await AssignedStops.FirstOrDefaultAsync(s => s.Id == key);

    public void Add(CollectedStop aggregate) => CollectedStops.Add(aggregate);

    public void Delete(CollectedStop aggregate) => CollectedStops.Remove(aggregate);

    async Task<CollectedStop?> IRepository<CollectedStop, StopId>.TryFindAsync(StopId key) =>
        await CollectedStops.FirstOrDefaultAsync(s => s.Id == key);

    public void Add(UnresolvedStop aggregate) => UnresolvedStops.Add(aggregate);

    public void Delete(UnresolvedStop aggregate) => UnresolvedStops.Remove(aggregate);

    async Task<UnresolvedStop?> IRepository<UnresolvedStop, StopId>.TryFindAsync(StopId key) =>
        await UnresolvedStops.FirstOrDefaultAsync(s => s.Id == key);

    public void Add(Team aggregate) => Teams.Add(aggregate);

    public void Delete(Team aggregate) => Teams.Remove(aggregate);

    async Task<Team?> IRepository<Team, TeamId>.TryFindAsync(TeamId key) =>
        await Teams.FirstOrDefaultAsync(t => t.Id == key);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddCampaigns().AddTeams().AddStops().AddStoredDomainEvents();
    }

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() =>
        (IRepository<TAggregate, TKey>)this;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries<StopBase>();

        var events = entities.SelectMany(e => e.Entity.NewEvents).ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

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

            await base.SaveChangesAsync(cancellationToken);

            foreach (var entry in entities)
            {
                entry.Entity.ClearEvents();
            }
        }

        return result;
    }
}

public class TreeCampaignContextFactory : IDesignTimeDbContextFactory<TreeCampaignContext>
{
    public TreeCampaignContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TreeCampaignContext>();
        var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new TreeCampaignContext(optionsBuilder.Options);
    }
}

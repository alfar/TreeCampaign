using System.Threading.Channels;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.BackgroundWorkers.Signals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Infrastructure;

public class TreeCampaignContext(DbContextOptions<TreeCampaignContext> options, ChannelWriter<EventDispatchSignal> eventDispatcher)
    : OutboxDbContext(options, eventDispatcher),
        ITreeCampaignUnitOfWork,
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

    public void Add(Campaign aggregate) => CollectionCampaigns.Add(aggregate);

    public void Delete(Campaign aggregate) => CollectionCampaigns.Remove(aggregate);

    async Task<Campaign?> IRepository<Campaign, CampaignId>.TryFindAsync(CampaignId key, CancellationToken cancellationToken) =>
        await CollectionCampaigns.FirstOrDefaultAsync(c => c.Id == key, cancellationToken);

    public void Add(UnassignedStop aggregate) => UnassignedStops.Add(aggregate);

    public void Delete(UnassignedStop aggregate) => UnassignedStops.Remove(aggregate);

    async Task<UnassignedStop?> IRepository<UnassignedStop, StopId>.TryFindAsync(StopId key, CancellationToken cancellationToken) =>
        await UnassignedStops.FirstOrDefaultAsync(s => s.Id == key, cancellationToken);

    public void Add(AssignedStop aggregate) => AssignedStops.Add(aggregate);

    public void Delete(AssignedStop aggregate) => AssignedStops.Remove(aggregate);

    public void Add(ReopenableStop aggregate) => ReopenableStops.Add(aggregate);

    public void Delete(ReopenableStop aggregate) => ReopenableStops.Remove(aggregate);

    async Task<ReopenableStop?> IRepository<ReopenableStop, StopId>.TryFindAsync(StopId key, CancellationToken cancellationToken) =>
        await ReopenableStops.FirstOrDefaultAsync(s => s.Id == key, cancellationToken);

    async Task<AssignedStop?> IRepository<AssignedStop, StopId>.TryFindAsync(StopId key, CancellationToken cancellationToken) =>
        await AssignedStops.FirstOrDefaultAsync(s => s.Id == key, cancellationToken);

    public void Add(CollectedStop aggregate) => CollectedStops.Add(aggregate);

    public void Delete(CollectedStop aggregate) => CollectedStops.Remove(aggregate);

    async Task<CollectedStop?> IRepository<CollectedStop, StopId>.TryFindAsync(StopId key, CancellationToken cancellationToken) =>
        await CollectedStops.FirstOrDefaultAsync(s => s.Id == key, cancellationToken);

    public void Add(UnresolvedStop aggregate) => UnresolvedStops.Add(aggregate);

    public void Delete(UnresolvedStop aggregate) => UnresolvedStops.Remove(aggregate);

    async Task<UnresolvedStop?> IRepository<UnresolvedStop, StopId>.TryFindAsync(StopId key, CancellationToken cancellationToken) =>
        await UnresolvedStops.FirstOrDefaultAsync(s => s.Id == key, cancellationToken);

    public void Add(Team aggregate) => Teams.Add(aggregate);

    public void Delete(Team aggregate) => Teams.Remove(aggregate);

    async Task<Team?> IRepository<Team, TeamId>.TryFindAsync(TeamId key, CancellationToken cancellationToken) =>
        await Teams.FirstOrDefaultAsync(t => t.Id == key, cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddCampaigns().AddTeams().AddStops();
    }

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() =>
        (IRepository<TAggregate, TKey>)this;
}

public class TreeCampaignContextFactory : IDesignTimeDbContextFactory<TreeCampaignContext>
{
    public TreeCampaignContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TreeCampaignContext>();
        var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new TreeCampaignContext(optionsBuilder.Options, null!);
    }
}

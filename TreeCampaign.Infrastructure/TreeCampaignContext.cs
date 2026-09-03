using System.Threading.Channels;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.BackgroundWorkers.Signals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;
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
        IRepository<DeliveredStop, StopId>,
        IRepository<TeamBase, TeamId>,
        IRepository<WalkingTeam, TeamId>,
        IRepository<TrailerTeam, TeamId>
{
    public DbSet<Campaign> CollectionCampaigns { get; set; }
    internal DbSet<StopBase> Stops { get; set; }
    public DbSet<UnassignedStop> UnassignedStops { get; set; }
    public DbSet<AssignedStop> AssignedStops { get; set; }
    public DbSet<ReopenableStop> ReopenableStops { get; set; }
    public DbSet<CollectedStop> CollectedStops { get; set; }
    public DbSet<UnresolvedStop> UnresolvedStops { get; set; }
    public DbSet<DeliveredStop> DeliveredStops { get; set; }
    public DbSet<TeamBase> Teams { get; set; }
    public DbSet<WalkingTeam> WalkingTeams { get; set; }
    public DbSet<TrailerTeam> TrailerTeams { get; set; }

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

    public void Add(DeliveredStop aggregate) => DeliveredStops.Add(aggregate);

    public void Delete(DeliveredStop aggregate) => DeliveredStops.Remove(aggregate);

    async Task<DeliveredStop?> IRepository<DeliveredStop, StopId>.TryFindAsync(StopId key, CancellationToken cancellationToken) =>
        await DeliveredStops.FirstOrDefaultAsync(s => s.Id == key, cancellationToken);

    public void Add(TeamBase aggregate) => Teams.Add(aggregate);

    public void Delete(TeamBase aggregate) => Teams.Remove(aggregate);

    async Task<TeamBase?> IRepository<TeamBase, TeamId>.TryFindAsync(TeamId key, CancellationToken cancellationToken) =>
        await Teams.Include(t => t.Members).FirstOrDefaultAsync(t => t.Id == key, cancellationToken);

    public void Add(WalkingTeam aggregate) => WalkingTeams.Add(aggregate);

    public void Delete(WalkingTeam aggregate) => WalkingTeams.Remove(aggregate);

    async Task<WalkingTeam?> IRepository<WalkingTeam, TeamId>.TryFindAsync(TeamId key, CancellationToken cancellationToken) =>
        await WalkingTeams.Include(t => t.Members).FirstOrDefaultAsync(t => t.Id == key, cancellationToken);

    public void Add(TrailerTeam aggregate) => TrailerTeams.Add(aggregate);

    public void Delete(TrailerTeam aggregate) => TrailerTeams.Remove(aggregate);

    async Task<TrailerTeam?> IRepository<TrailerTeam, TeamId>.TryFindAsync(TeamId key, CancellationToken cancellationToken) =>
        await TrailerTeams.Include(t => t.Members).FirstOrDefaultAsync(t => t.Id == key, cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddCampaigns().AddTeams().AddTeamMembers().AddStops();
    }

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() =>
        (IRepository<TAggregate, TKey>)this;
}

public class TreeCampaignContextFactory : IDesignTimeDbContextFactory<TreeCampaignContext>
{
    public TreeCampaignContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TreeCampaignContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TreeCampaign;Trusted_Connection=True;");

        return new TreeCampaignContext(optionsBuilder.Options, null!);
    }
}

using Microsoft.EntityFrameworkCore;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure.Queries;
using TreeCampaign.Infrastructure.ValueConverters;
using TeamMember = TreeCampaign.Domain.Teams.ValueObjects.TeamMember;
using TeamStatus = TreeCampaign.Domain.Teams.TeamStatus;

public class ProjectionContext(DbContextOptions<ProjectionContext> options) : DbContext(options)
{
    public class StopProjection
    {
        public required StopId Id { get; init; }
        public required Address Address { get; init; }
        public required TreeCount Amount { get; init; }
        public required string StopType { get; init; }
        public TeamId? AssignedTeamId { get; init; }
        public ReasonText? UnresolvedReason { get; init; }

        public static StopProjection From(UnassignedStop stop) =>
            new()
            {
                Id = stop.Id,
                Address = stop.Address,
                Amount = stop.Amount,
                StopType = IStopQueries.State.Unassigned.ToString(),
            };

        public static StopProjection From(AssignedStop stop) =>
            new()
            {
                Id = stop.Id,
                Address = stop.Address,
                Amount = stop.Amount,
                StopType = IStopQueries.State.Assigned.ToString(),
                AssignedTeamId = stop.AssignedTeamId,
            };

        public static StopProjection From(CollectedStop stop) =>
            new()
            {
                Id = stop.Id,
                Address = stop.Address,
                Amount = stop.Amount,
                StopType = IStopQueries.State.Collected.ToString(),
            };

        public static StopProjection From(UnresolvedStop stop) =>
            new()
            {
                Id = stop.Id,
                Address = stop.Address,
                Amount = stop.Amount,
                StopType = IStopQueries.State.Unresolved.ToString(),
                AssignedTeamId = stop.UnresolvedByTeamId,
                UnresolvedReason = stop.UnresolvedReason,
            };
    }

    public class CampaignProjection
    {
        public required CampaignId Id { get; init; }
        public required CampaignSeason Season { get; init; }
        public TerritoryRef? TerritoryId { get; init; }
    }

    public class TeamProjection
    {
        public required TeamId Id { get; init; }
        public required TeamName Name { get; init; }
        public TeamStatus Status { get; init; } = TeamStatus.Active;
        public IReadOnlyCollection<TeamMember> Members { get; init; } = [];
    }

    private DbSet<CampaignProjection> CampaignProjections { get; set; }

    private DbSet<StopProjection> StopProjections { get; set; }
    private DbSet<TeamProjection> TeamProjections { get; set; }

    public IQueryable<CampaignProjection> Campaigns => CampaignProjections.AsNoTracking();

    public IQueryable<StopProjection> Stops => StopProjections.AsNoTracking();

    public IQueryable<TeamProjection> Teams => TeamProjections.AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StopProjection>().HasKey(s => s.Id);

        modelBuilder
            .Entity<StopProjection>()
            .ToTable("Stops")
            .Property<CampaignId>("CampaignId")
            .HasConversion(new CampaignIdValueConverter());

        modelBuilder
            .Entity<StopProjection>()
            .Property(s => s.Id)
            .HasConversion(new StopIdValueConverter());

        modelBuilder
            .Entity<StopProjection>()
            .ComplexProperty(
                s => s.Address,
                a =>
                {
                    a.Property(p => p.DisplayName).HasColumnName("AddressDisplayName");
                    a.Property(p => p.Latitude).HasColumnName("AddressLatitude");
                    a.Property(p => p.Longitude).HasColumnName("AddressLongitude");
                    a.Property(p => p.StreetSectionId).HasColumnName("StreetSectionId").HasConversion(new StreetSectionRefValueConverter());
                }
            );
        modelBuilder
            .Entity<StopProjection>()
            .Property(s => s.Amount)
            .HasConversion(new TreeCountValueConverter());
        modelBuilder.Entity<StopProjection>().Property(s => s.StopType);
        modelBuilder
            .Entity<StopProjection>()
            .Property(s => s.AssignedTeamId)
            .HasConversion(new NullableTeamIdValueConverter());
        modelBuilder
            .Entity<StopProjection>()
            .Property(s => s.UnresolvedReason)
            .HasConversion(new NullableReasonTextValueConverter());

        modelBuilder.Entity<CampaignProjection>().HasKey(c => c.Id);
        modelBuilder
            .Entity<CampaignProjection>()
            .Property(c => c.Id)
            .HasConversion(new CampaignIdValueConverter());
        modelBuilder
            .Entity<CampaignProjection>()
            .Property(c => c.Season)
            .HasConversion(new CampaignSeasonValueConverter());
        modelBuilder
            .Entity<CampaignProjection>()
            .Property(c => c.TerritoryId)
            .HasConversion(new NullableTerritoryRefValueConverter());
        modelBuilder.Entity<CampaignProjection>().ToTable("Campaigns");

        modelBuilder.Entity<TeamProjection>().HasKey(t => t.Id);
        modelBuilder
            .Entity<TeamProjection>()
            .Property(t => t.Id)
            .HasConversion(new TeamIdValueConverter());

        modelBuilder
            .Entity<TeamProjection>()
            .Property(t => t.Name)
            .HasConversion(new TeamNameValueConverter());
        modelBuilder
            .Entity<TeamProjection>()
            .Property<CampaignId>("CampaignId")
            .HasConversion(new CampaignIdValueConverter());
        modelBuilder.Entity<TeamProjection>().ToTable("Teams");
        modelBuilder
            .Entity<TeamProjection>()
            .Property(t => t.Status)
            .HasConversion<byte>()
            .HasDefaultValue(TeamStatus.Active);

        modelBuilder.Entity<TeamProjection>().OwnsMany(t => t.Members, m =>
        {
            m.ToTable("TeamMembers");
            m.HasKey(x => x.Id);
            m.WithOwner().HasForeignKey("TeamId");
        });
    }

    public override int SaveChanges() =>
        throw new InvalidOperationException("ProjectionContext is read-only");

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("ProjectionContext is read-only");
}

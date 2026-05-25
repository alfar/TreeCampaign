using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Repository.ValueConverters;

internal static class StopConfiguration
{
    public static ModelBuilder AddStops(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StopBase>().Configure();

        modelBuilder
            .Entity<UnassignedStop>()
            .Property<Guid?>("AssignedTeamId")
            .HasValueGenerator<NullValueGenerator<Guid>>()
            .HasColumnName("AssignedTeamId");

        modelBuilder
            .Entity<AssignedStop>()
            .Property(s => s.AssignedTeamId)
            .HasConversion(new TeamIdValueConverter())
            .HasColumnName("AssignedTeamId");

        modelBuilder
            .Entity<CollectedStop>()
            .Property(s => s.CollectedByTeamId)
            .HasConversion(new TeamIdValueConverter())
            .HasColumnName("AssignedTeamId");

        modelBuilder
            .Entity<UnresolvedStop>()
            .Property(s => s.UnresolvedByTeamId)
            .HasConversion(new TeamIdValueConverter())
            .HasColumnName("AssignedTeamId");

        modelBuilder
            .Entity<UnresolvedStop>()
            .Property(s => s.UnresolvedReason)
            .HasConversion(new ReasonTextValueConverter())
            .HasColumnName("UnresolvedReason");

        return modelBuilder;
    }

    public static EntityTypeBuilder<StopBase> Configure(this EntityTypeBuilder<StopBase> builder)
    {
        builder.ToTable("Stops");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).HasConversion(new StopIdValueConverter());
        builder.ComplexProperty(
            s => s.Address,
            a =>
            {
                a.Property(p => p.DisplayName).HasColumnName("AddressDisplayName");
                a.Property(p => p.Latitude).HasColumnName("AddressLatitude");
                a.Property(p => p.Longitude).HasColumnName("AddressLongitude");
            }
        );

        builder.Property(s => s.CampaignId).HasConversion(new CampaignIdValueConverter());
        builder.Property(s => s.Amount).HasConversion(new TreeCountValueConverter());

        builder
            .HasDiscriminator<string>("StopType")
            .HasValue<UnassignedStop>("Unassigned")
            .HasValue<AssignedStop>("Assigned")
            .HasValue<CollectedStop>("Collected")
            .HasValue<UnresolvedStop>("Unresolved");

        return builder;
    }
}

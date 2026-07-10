using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeCampaign.Domain.TeamMembers;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Infrastructure.ValueConverters;
using TeamStatus = TreeCampaign.Domain.Teams.TeamStatus;

internal static class TeamConfiguration
{
    public static ModelBuilder AddTeams(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeamBase>().ToTable("Teams").Configure();

        modelBuilder.Entity<TrailerTeam>()
            .Property(t => t.IsTrailerFull)
            .HasDefaultValue(false);

        return modelBuilder;
    }

    public static EntityTypeBuilder<TeamBase> Configure(this EntityTypeBuilder<TeamBase> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasConversion(new TeamIdValueConverter());
        builder.Property(t => t.CampaignId).HasConversion(new CampaignIdValueConverter());
        builder.Property(t => t.Name).HasConversion(new TeamNameValueConverter());
        builder.Property(t => t.Status)
            .HasConversion<byte>()
            .HasDefaultValue(TeamStatus.Active);

        builder.HasMany<TeamMember>().WithOne().HasForeignKey(s => s.TeamId);

        builder.Navigation(t => t.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasDiscriminator<string>("TeamKind")
            .HasValue<WalkingTeam>("Walking")
            .HasValue<TrailerTeam>("Trailer");

        return builder;
    }
}

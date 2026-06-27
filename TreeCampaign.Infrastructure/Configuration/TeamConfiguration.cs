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
        modelBuilder.Entity<Team>().ToTable("Teams").Configure();
        return modelBuilder;
    }

    public static EntityTypeBuilder<Team> Configure(this EntityTypeBuilder<Team> builder)
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

        return builder;
    }
}

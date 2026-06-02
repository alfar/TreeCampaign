using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeCampaign.Domain.Teams;
using TreeCampaign.InfraStructure.ValueConverters;

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
        return builder;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.ValueObjects;
using TreeCampaign.Infrastructure.ValueConverters;

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

        builder.OwnsMany(t => t.Members, m =>
        {
            m.ToTable("TeamMembers");
            m.HasKey(x => x.Id);
            m.Property(x => x.Id);
            m.Property(x => x.Name);
            m.Property(x => x.ScoutRelativeName);
            m.Property(x => x.PhoneNumber);
            m.WithOwner().HasForeignKey("TeamId");
        });
        builder.Navigation(t => t.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        return builder;
    }
}

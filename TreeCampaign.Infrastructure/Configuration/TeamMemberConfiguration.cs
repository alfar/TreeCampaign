using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeCampaign.Domain.TeamMembers;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Infrastructure.ValueConverters;

internal static class TeamMemberConfiguration
{
    public static ModelBuilder AddTeamMembers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeamMember>().ToTable("TeamMembers").Configure();
        return modelBuilder;
    }

    public static EntityTypeBuilder<TeamMember> Configure(this EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasConversion(new TeamMemberIdValueConverter());
        builder.Property(x => x.Name);
        builder.Property(x => x.ScoutRelativeName);
        builder.Property(x => x.PhoneNumber);
        builder.Property(x => x.TeamId).HasConversion(new TeamIdValueConverter());

        builder.HasOne<TeamBase>().WithMany(n => n.Members).HasForeignKey(s => s.TeamId);

        return builder;
    }
}

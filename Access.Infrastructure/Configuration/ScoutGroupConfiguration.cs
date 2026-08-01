using Access.Domain.ScoutGroups;
using Access.Domain.Users;
using Access.Infrastructure.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Access.Infrastructure.Configuration;

internal static class ScoutGroupConfiguration
{
    public static ModelBuilder AddScoutGroups(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScoutGroup>().ToTable("ScoutGroups").Configure();
        return modelBuilder;
    }

    public static void Configure(this EntityTypeBuilder<ScoutGroup> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).HasConversion(new ScoutGroupIdValueConverter());
        builder.Property(g => g.Name).IsRequired().HasMaxLength(100);
        builder.HasMany<User>().WithOne().HasForeignKey(u => u.ScoutGroupId);
    }
}

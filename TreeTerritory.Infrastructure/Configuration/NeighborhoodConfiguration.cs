using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.Territories;
using TreeTerritory.InfraStructure.ValueConverters;

namespace TreeTerritory.InfraStructure.Configuration;

internal static class NeighborhoodConfiguration
{
    public static ModelBuilder AddNeighborhoods(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Neighborhood>().ToTable("Neighborhoods").Configure();
        return modelBuilder;
    }

    public static void Configure(this EntityTypeBuilder<Neighborhood> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(new NeighborhoodIdValueConverter());
        builder.Property(c => c.TerritoryId).HasConversion(new TerritoryIdValueConverter());
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.HasOne<Territory>().WithMany().HasForeignKey(n => n.TerritoryId);
        builder.HasMany<StreetSection>().WithOne().HasForeignKey(s => s.NeighborhoodId);

        builder.Navigation(n => n.StreetSections).HasField("_streetSections").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

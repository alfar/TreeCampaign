using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.Territories;
using TreeTerritory.Infrastructure.ValueConverters;

namespace TreeTerritory.Infrastructure.Configuration;

internal static class TerritoryConfiguration
{
    public static ModelBuilder AddTerritories(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Territory>().ToTable("Territories").Configure();
        return modelBuilder;
    }

    public static void Configure(this EntityTypeBuilder<Territory> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(new TerritoryIdValueConverter());
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.DefaultZipCode).HasConversion(new NullableZipCodeValueConverter()).HasColumnName("DefaultZipCode");
        builder.Property(c => c.ScoutGroupId).HasConversion(new ScoutGroupRefValueConverter()).HasColumnName("ScoutGroupId");
        builder.HasMany<Neighborhood>().WithOne().HasForeignKey(s => s.TerritoryId);
    }
}

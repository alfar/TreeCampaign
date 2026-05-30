using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.Territories;
using TreeTerritory.Repository.ValueConverters;

namespace TreeTerritory.Repository.Configuration;

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
        builder.HasMany<Neighborhood>().WithOne().HasForeignKey(s => s.TerritoryId);
    }
}

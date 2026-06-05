
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Infrastructure.ValueConverters;

namespace TreeTerritory.Infrastructure.Configuration;

internal static class StreetSectionConfiguration
{
    public static ModelBuilder AddStreetSections(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StreetSection>().ToTable("StreetSections").Configure();
        return modelBuilder;
    }

    public static void Configure(this EntityTypeBuilder<StreetSection> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(new StreetSectionIdValueConverter());
        builder.Property(c => c.NeighborhoodId).HasConversion(new NeighborhoodIdValueConverter());
        builder.Property(c => c.StreetId).HasConversion(new StreetIdValueConverter());
        builder.Property(c => c.StartHouseNumber).HasConversion(new HouseNumberValueConverter());
        builder.Property(c => c.EndHouseNumber).HasConversion(new HouseNumberValueConverter());
        builder.Property(c => c.Direction).HasConversion<byte>();
        builder.Property(c => c.SortOrder);
        
        builder.HasOne<Neighborhood>().WithMany(n => n.StreetSections).HasForeignKey(s => s.NeighborhoodId);
    }
}
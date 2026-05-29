
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeTerritory.Domain.Streets;
using TreeTerritory.Repository.ValueConverters;

internal static class StreetConfiguration
{
    public static ModelBuilder AddStreets(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Street>().ToTable("Streets").Configure();
        return modelBuilder;
    }

    public static void Configure(this EntityTypeBuilder<Street> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(new StreetIdValueConverter());
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.ZipCode).IsRequired().HasConversion(new ZipCodeValueConverter());
    }
}
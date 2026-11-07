using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Infrastructure.ValueConverters;

internal static class CampaignConfiguration
{
    public static ModelBuilder AddCampaigns(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campaign>().ToTable("Campaigns").Configure();
        return modelBuilder;
    }

    public static void Configure(this EntityTypeBuilder<Campaign> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(new CampaignIdValueConverter());
        builder.Property(c => c.Season).HasConversion(new CampaignSeasonValueConverter());
        builder.Property(c => c.TerritoryId).HasConversion(new NullableTerritoryRefValueConverter());
        builder.Property(c => c.ScoutGroupId).HasConversion(new ScoutGroupRefValueConverter());
        builder.Property(c => c.CollectionDate);
        builder.Property(c => c.PricePerTree).HasConversion(new NullableMoneyAmountValueConverter()).HasPrecision(18, 2);
        builder.Property(c => c.Title).HasMaxLength(200);
        builder.Property(c => c.LastOrderDateTime);
    }
}

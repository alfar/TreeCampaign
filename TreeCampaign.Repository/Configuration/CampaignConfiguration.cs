using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Repository.ValueConverters;

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
    }
}

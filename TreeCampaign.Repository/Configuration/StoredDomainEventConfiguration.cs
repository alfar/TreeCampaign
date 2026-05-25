using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeCampaign.Repository.Events;
using TreeCampaign.Repository.ValueConverters;

internal static class StoredDomainEventConfiguration
{
    public static ModelBuilder AddStoredDomainEvents(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoredDomainEvent>().Configure();

        return modelBuilder;
    }

    public static EntityTypeBuilder<StoredDomainEvent> Configure(
        this EntityTypeBuilder<StoredDomainEvent> builder
    )
    {
        builder.ToTable("StoredDomainEvents").HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        return builder;
    }
}

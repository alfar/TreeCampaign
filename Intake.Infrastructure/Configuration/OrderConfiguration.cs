using Intake.Domain.Orders;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Intake.Infrastructure.Configuration;

internal static class OrderConfiguration
{
    public static ModelBuilder AddOrders(this ModelBuilder modelBuilder)
    {
        var orderBase = modelBuilder.Entity<OrderBase>();

        orderBase.ToTable("Orders").HasKey(o => o.Id);
        orderBase.HasDiscriminator<string>("OrderType")
            .HasValue<IncomingOrder>("Incoming")
            .HasValue<UnwashedOrder>("Unwashed")
            .HasValue<WashedOrder>("Washed")
            .HasValue<OutOfBoundsOrder>("OutOfBounds")
            .HasValue<ValidatedOrder>("Validated")
            .HasValue<TransferredOrder>("Transferred")
            .HasValue<SettledOrder>("Settled");

        orderBase.Property(o => o.Id).HasConversion(new OrderIdValueConverter());
        orderBase.Property(o => o.CampaignId).HasConversion(new CampaignRefValueConverter());
        orderBase.ComplexProperty(o => o.Sender, s =>
        {
            s.Property(p => p.Name).HasColumnName("SenderName");
            s.Property(p => p.PhoneNumber).HasColumnName("SenderPhoneNumber");
        });
        orderBase.Property(o => o.Amount).HasConversion(new MoneyAmountValueConverter());
        orderBase.Property(o => o.TransactionId).HasConversion(new TransactionIdValueConverter());
        orderBase.HasIndex(o => o.TransactionId).IsUnique().HasFilter("TransactionId IS NOT NULL");

        modelBuilder.Entity<OutOfBoundsOrder>()
            .Property(o => o.StreetId)
            .HasColumnName("StreetId")
            .HasConversion(new StreetRefValueConverter());

        modelBuilder.Entity<WashedOrder>()
            .Property(o => o.StreetId)
            .HasColumnName("StreetId")
            .HasConversion(new StreetRefValueConverter());

        modelBuilder.Entity<ValidatedOrder>()
            .Property(o => o.StreetId)
            .HasColumnName("StreetId")
            .HasConversion(new StreetRefValueConverter());

        modelBuilder.Entity<TransferredOrder>()
            .Property(o => o.StreetId)
            .HasColumnName("StreetId")
            .HasConversion(new StreetRefValueConverter());

        modelBuilder.Entity<SettledOrder>()
            .Property(o => o.StreetId)
            .HasColumnName("StreetId")
            .HasConversion(new StreetRefValueConverter());

        modelBuilder.Entity<WashedOrder>()
            .Property(o => o.StreetSectionId)
            .HasColumnName("StreetSectionId")
            .HasConversion(new StreetSectionRefValueConverter());

        modelBuilder.Entity<ValidatedOrder>()
            .Property(o => o.StreetSectionId)
            .HasColumnName("StreetSectionId")
            .HasConversion(new StreetSectionRefValueConverter());

        modelBuilder.Entity<WashedOrder>()
            .Property(o => o.NeighborhoodId)
            .HasColumnName("NeighborhoodId")
            .HasConversion(new NeighborhoodRefValueConverter());

        modelBuilder.Entity<ValidatedOrder>()
            .Property(o => o.NeighborhoodId)
            .HasColumnName("NeighborhoodId")
            .HasConversion(new NeighborhoodRefValueConverter());

        modelBuilder.Entity<WashedOrder>()
            .Property(o => o.HouseNumber)
            .HasColumnName("HouseNumber")
            .HasConversion(new HouseNumberValueConverter());

        modelBuilder.Entity<OutOfBoundsOrder>()
            .Property(o => o.HouseNumber)
            .HasColumnName("HouseNumber")
            .HasConversion(new HouseNumberValueConverter());

        modelBuilder.Entity<ValidatedOrder>()
            .Property(o => o.HouseNumber)
            .HasColumnName("HouseNumber")
            .HasConversion(new HouseNumberValueConverter());

        modelBuilder.Entity<TransferredOrder>()
            .Property(o => o.HouseNumber)
            .HasColumnName("HouseNumber")
            .HasConversion(new HouseNumberValueConverter());

        modelBuilder.Entity<SettledOrder>()
            .Property(o => o.HouseNumber)
            .HasColumnName("HouseNumber")
            .HasConversion(new HouseNumberValueConverter());

        modelBuilder.Entity<ValidatedOrder>()
            .Property(o => o.Longitude);

        modelBuilder.Entity<ValidatedOrder>()
            .Property(o => o.Latitude);

        modelBuilder.Entity<TransferredOrder>()
            .Property(o => o.TerritoryId)
            .HasColumnName("TerritoryId")
            .HasConversion(new TerritoryRefValueConverter());

        modelBuilder.Entity<SettledOrder>()
            .Property(o => o.TerritoryId)
            .HasColumnName("TerritoryId")
            .HasConversion(new TerritoryRefValueConverter());

        return modelBuilder;
    }
}

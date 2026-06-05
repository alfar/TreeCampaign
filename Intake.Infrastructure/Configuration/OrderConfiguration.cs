using Intake.Domain.Orders;
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
            .HasValue<ValidatedOrder>("Validated");

        orderBase.Property(o => o.Id).HasConversion(new OrderIdValueConverter());
        orderBase.Property(o => o.CampaignId).HasConversion(new CampaignRefValueConverter());
        orderBase.ComplexProperty(o => o.Sender, s =>
        {
            s.Property(p => p.Name).HasColumnName("SenderName");
            s.Property(p => p.PhoneNumber).HasColumnName("SenderPhoneNumber");
        });
        orderBase.Property(o => o.Amount).HasConversion(new MoneyAmountValueConverter());

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

        modelBuilder.Entity<WashedOrder>()
            .Property(o => o.StreetSectionId)
            .HasConversion(new StreetSectionRefValueConverter());

        modelBuilder.Entity<ValidatedOrder>()
            .Property(o => o.StreetSectionId)
            .HasConversion(new StreetSectionRefValueConverter());

        modelBuilder.Entity<WashedOrder>()
            .Property(o => o.NeighborhoodId)
            .HasConversion(new NeighborhoodRefValueConverter());

        modelBuilder.Entity<ValidatedOrder>()
            .Property(o => o.NeighborhoodId)
            .HasConversion(new NeighborhoodRefValueConverter());

        return modelBuilder;
    }
}

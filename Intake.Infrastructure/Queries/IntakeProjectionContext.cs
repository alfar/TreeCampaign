using Intake.InfraStructure.Queries;
using Intake.InfraStructure.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Intake.InfraStructure;

public class IntakeProjectionContext(DbContextOptions<IntakeProjectionContext> options) : DbContext(options)
{
    private DbSet<OrderProjection> OrderProjections { get; set; }

    public IQueryable<OrderProjection> Orders => OrderProjections.AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderProjection>().ToTable("Orders").HasKey(o => o.Id);
        modelBuilder.Entity<OrderProjection>().Property(o => o.Id).HasColumnName("Id").HasConversion(new OrderIdValueConverter());
        modelBuilder.Entity<OrderProjection>().Property(o => o.OrderType).HasColumnName("OrderType");
        modelBuilder.Entity<OrderProjection>().Property(o => o.CampaignId).HasColumnName("CampaignId").HasConversion(new CampaignRefValueConverter());
        modelBuilder.Entity<OrderProjection>().Property(o => o.SenderName).HasColumnName("SenderName");
        modelBuilder.Entity<OrderProjection>().Property(o => o.SenderPhoneNumber).HasColumnName("SenderPhoneNumber");
        modelBuilder.Entity<OrderProjection>().Property(o => o.Amount).HasColumnName("Amount").HasConversion(new MoneyAmountValueConverter());
        modelBuilder.Entity<OrderProjection>().Property(o => o.OrderDate).HasColumnName("OrderDate");
        modelBuilder.Entity<OrderProjection>().Property(o => o.Message).HasColumnName("Message");
        modelBuilder.Entity<OrderProjection>().Property(o => o.WashedStreet).HasColumnName("WashedStreet");
        modelBuilder.Entity<OrderProjection>().Property(o => o.WashedHouseNumber).HasColumnName("WashedHouseNumber");
        modelBuilder.Entity<OrderProjection>().Property(o => o.WashedZipCode).HasColumnName("WashedZipCode");
        modelBuilder.Entity<OrderProjection>().Property(o => o.StreetId).HasColumnName("StreetId").HasConversion(new NullableStreetRefValueConverter());
        modelBuilder.Entity<OrderProjection>().Property(o => o.StreetSectionId).HasColumnName("StreetSectionId").HasConversion(new NullableStreetSectionRefValueConverter());
        modelBuilder.Entity<OrderProjection>().Property(o => o.NeighborhoodId).HasColumnName("NeighborhoodId").HasConversion(new NullableNeighborhoodRefValueConverter());
    }

    public override int SaveChanges() =>
        throw new InvalidOperationException("IntakeProjectionContext is read-only.");

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("IntakeProjectionContext is read-only.");
}

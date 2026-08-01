using System.Threading.Channels;
using Common.Infrastructure;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.BackgroundWorkers.Signals;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Intake.Infrastructure;

public class IntakeContext(DbContextOptions<IntakeContext> options, ChannelWriter<EventDispatchSignal> eventDispatcher)
    : OutboxDbContext(options, eventDispatcher),
      IIntakeUnitOfWork,
      IRepository<IncomingOrder, OrderId>,
      IRepository<UnwashedOrder, OrderId>,
      IRepository<WashedOrder, OrderId>,
      IRepository<OutOfBoundsOrder, OrderId>,
      IRepository<ValidatedOrder, OrderId>,
      IRepository<TransferredOrder, OrderId>,
      IRepository<SettledOrder, OrderId>
{
    internal DbSet<OrderBase> Orders { get; set; }
    public DbSet<IncomingOrder> IncomingOrders { get; set; }
    public DbSet<UnwashedOrder> UnwashedOrders { get; set; }
    public DbSet<WashedOrder> WashedOrders { get; set; }
    public DbSet<OutOfBoundsOrder> OutOfBoundsOrders { get; set; }
    public DbSet<ValidatedOrder> ValidatedOrders { get; set; }
    public DbSet<TransferredOrder> TransferredOrders { get; set; }
    public DbSet<SettledOrder> SettledOrders { get; set; }

    public IQueryable<OrderBase> GetUnvalidatedOrdersByCampaign(CampaignRef campaignId) =>
        Orders.Where(o => !(o is ValidatedOrder) && o.CampaignId == campaignId);

    public IQueryable<OrderBase> GetUnvalidatedOrders() =>
        Orders.Where(o => !(o is ValidatedOrder));

    public async Task<OrderBase?> FindOrderByIdAsync(OrderId orderId, CancellationToken cancellationToken) =>
        await Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

    public async Task<IReadOnlySet<TransactionId>> GetExistingTransactionIdsAsync(IEnumerable<TransactionId> transactionIds, CancellationToken cancellationToken)
    {
        var ids = transactionIds.ToList();
        var existing = await Orders
            .Where(o => o.TransactionId != null && ids.Contains(o.TransactionId))
            .Select(o => o.TransactionId!)
            .ToListAsync(cancellationToken);

        return existing.ToHashSet();
    }

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() =>
        (IRepository<TAggregate, TKey>)this;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddOrders();
    }

    public void Add(IncomingOrder aggregate) => IncomingOrders.Add(aggregate);
    public void Delete(IncomingOrder aggregate) => IncomingOrders.Remove(aggregate);
    async Task<IncomingOrder?> IRepository<IncomingOrder, OrderId>.TryFindAsync(OrderId key, CancellationToken ct) =>
        await IncomingOrders.FirstOrDefaultAsync(o => o.Id == key, ct);

    public void Add(UnwashedOrder aggregate) => UnwashedOrders.Add(aggregate);
    public void Delete(UnwashedOrder aggregate) => UnwashedOrders.Remove(aggregate);
    async Task<UnwashedOrder?> IRepository<UnwashedOrder, OrderId>.TryFindAsync(OrderId key, CancellationToken ct) =>
        await UnwashedOrders.FirstOrDefaultAsync(o => o.Id == key, ct);

    public void Add(WashedOrder aggregate) => WashedOrders.Add(aggregate);
    public void Delete(WashedOrder aggregate) => WashedOrders.Remove(aggregate);
    async Task<WashedOrder?> IRepository<WashedOrder, OrderId>.TryFindAsync(OrderId key, CancellationToken ct) =>
        await WashedOrders.FirstOrDefaultAsync(o => o.Id == key, ct);

    public void Add(OutOfBoundsOrder aggregate) => OutOfBoundsOrders.Add(aggregate);
    public void Delete(OutOfBoundsOrder aggregate) => OutOfBoundsOrders.Remove(aggregate);
    async Task<OutOfBoundsOrder?> IRepository<OutOfBoundsOrder, OrderId>.TryFindAsync(OrderId key, CancellationToken ct) =>
        await OutOfBoundsOrders.FirstOrDefaultAsync(o => o.Id == key, ct);

    public void Add(ValidatedOrder aggregate) => ValidatedOrders.Add(aggregate);
    public void Delete(ValidatedOrder aggregate) => ValidatedOrders.Remove(aggregate);
    async Task<ValidatedOrder?> IRepository<ValidatedOrder, OrderId>.TryFindAsync(OrderId key, CancellationToken ct) =>
        await ValidatedOrders.FirstOrDefaultAsync(o => o.Id == key, ct);

    async Task<TransferredOrder?> IRepository<TransferredOrder, OrderId>.TryFindAsync(OrderId key, CancellationToken cancellationToken) => 
        await TransferredOrders.FirstOrDefaultAsync(o => o.Id == key, cancellationToken);

    public void Add(TransferredOrder aggregate) => TransferredOrders.Add(aggregate);

    public void Delete(TransferredOrder aggregate) => TransferredOrders.Remove(aggregate);

    async Task<SettledOrder?> IRepository<SettledOrder, OrderId>.TryFindAsync(OrderId key, CancellationToken cancellationToken) =>
        await SettledOrders.FirstOrDefaultAsync(o => o.Id == key, cancellationToken);

    public void Add(SettledOrder aggregate) => SettledOrders.Add(aggregate);

    public void Delete(SettledOrder aggregate) => SettledOrders.Remove(aggregate);
}

public class IntakeContextFactory : IDesignTimeDbContextFactory<IntakeContext>
{
    public IntakeContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IntakeContext>();
        var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        return new IntakeContext(optionsBuilder.Options, null!);
    }
}

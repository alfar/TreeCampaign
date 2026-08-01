using Common.Infrastructure.Abstractions;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Infrastructure;

public interface IIntakeUnitOfWork : IUnitOfWork
{
    IQueryable<OrderBase> GetUnvalidatedOrdersByCampaign(CampaignRef campaignId);
    IQueryable<OrderBase> GetUnvalidatedOrders();
    IQueryable<TransferredOrder> GetTransferredOrdersByTerritory(CampaignRef campaignId, TerritoryRef territoryId);
    Task<OrderBase?> FindOrderByIdAsync(OrderId orderId, CancellationToken cancellationToken);
    Task<IReadOnlySet<TransactionId>> GetExistingTransactionIdsAsync(IEnumerable<TransactionId> transactionIds, CancellationToken cancellationToken);
}

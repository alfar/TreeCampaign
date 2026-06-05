using Common.Infrastructure.Abstractions;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Infrastructure;

public interface IIntakeUnitOfWork : IUnitOfWork
{
    IQueryable<OrderBase> GetUnvalidatedOrdersByCampaign(CampaignRef campaignId);
    IQueryable<OrderBase> GetUnvalidatedOrders();
    Task<OrderBase?> FindOrderByIdAsync(OrderId orderId, CancellationToken cancellationToken);
}

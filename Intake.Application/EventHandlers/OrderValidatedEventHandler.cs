using System.Threading.Channels;
using Common.Domain.Abstractions;
using Common.InfraStructure.Abstractions;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Domain.Orders;
using Intake.Domain.Orders.Events;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Infrastructure;

namespace Intake.Application.EventHandlers;

public class OrderValidatedEventHandler(IIntakeUnitOfWork intakeUnitOfWork, ITreeCampaignUnitOfWork treeCampaignUnitOfWork) : IDomainEventHandler<OrderValidated>
{
    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent is OrderValidated orderValidated)
        {
            var order = await intakeUnitOfWork.GetRepository<ValidatedOrder, OrderId>().TryFindAsync(OrderId.From(domainEvent.AggregateId), cancellationToken);

            if (order is not null)
            {
                var stop = UnassignedStop.Create(CampaignId.From(order.CampaignId.Value), new Address("blah", 0, 0), TreeCount.From((int)Math.Floor(order.Amount.Value / 40)));

                treeCampaignUnitOfWork.GetRepository<UnassignedStop, StopId>().Add(stop);

                await treeCampaignUnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        throw new ArgumentException("Invalid event type", nameof(domainEvent));
    }
}
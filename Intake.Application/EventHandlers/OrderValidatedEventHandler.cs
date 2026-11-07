using Common.Domain.Abstractions;
using Common.InfraStructure.Abstractions;
using Intake.Domain.Orders;
using Intake.Domain.Orders.Events;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Infrastructure;
using TreeTerritory.Domain.Streets;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Infrastructure;

namespace Intake.Application.EventHandlers;

public class OrderValidatedEventHandler(IIntakeUnitOfWork intakeUnitOfWork, ITreeTerritoryUnitOfWork treeTerritoryUnitOfWork, ITreeCampaignUnitOfWork treeCampaignUnitOfWork) : IDomainEventHandler<OrderValidated>
{
    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent is OrderValidated)
        {
            var order = await intakeUnitOfWork.GetRepository<ValidatedOrder, OrderId>().TryFindAsync(OrderId.From(domainEvent.AggregateId), cancellationToken);

            if (order is not null)
            {
                var street = await treeTerritoryUnitOfWork.GetRepository<Street, StreetId>().TryFindAsync(StreetId.From(order.StreetId.Value));

                var campaignId = CampaignId.From(order.CampaignId.Value);
                var campaign = await treeCampaignUnitOfWork.GetRepository<Campaign, CampaignId>().TryFindAsync(campaignId, cancellationToken);

                if (street is not null && campaign is not null)
                {
                    var stop = UnassignedStop.Create(campaignId, new Address($"{street.Name} {order.HouseNumber}, {street.ZipCode}", order.Latitude, order.Longitude, TreeCampaign.Domain.ExternalReferences.StreetSectionRef.From(order.StreetSectionId.Value)), TreeCount.From(campaign.CalculateTreeCount(order.Amount.Value)));

                    treeCampaignUnitOfWork.GetRepository<UnassignedStop, StopId>().Add(stop);

                    await treeCampaignUnitOfWork.SaveChangesAsync(cancellationToken);

                    return;
                }
            }
        }

        throw new ArgumentException("Invalid event type", nameof(domainEvent));
    }
}
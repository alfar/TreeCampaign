using System.Threading.Channels;
using Common.Domain.Abstractions;
using Common.InfraStructure.Abstractions;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Domain.ExternalReferences;
using TreeTerritory.Domain.StreetSections.Events;
using TreeTerritory.Infrastructure.Queries;
using TerritoryRef = TreeCampaign.Domain.ExternalReferences.TerritoryRef;

namespace Intake.Application.EventHandlers;

public class StreetSectionCreatedEventHandler : IDomainEventHandler<StreetSectionCreated>
{
    private readonly ICampaignQueries _campaignQueries;
    private readonly INeighborhoodQueries _neighborhoodQueries;
    private readonly ChannelWriter<ValidationSignalBase> _writer;

    public StreetSectionCreatedEventHandler(ICampaignQueries campaignQueries, INeighborhoodQueries neighborhoodQueries, ChannelWriter<ValidationSignalBase> writer)
    {
        _campaignQueries = campaignQueries;
        _neighborhoodQueries = neighborhoodQueries;
        _writer = writer;
    }

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent is StreetSectionCreated streetSectionCreated)
        {
            var neighborhood = await _neighborhoodQueries.GetByIdAsync(streetSectionCreated.NeighborhoodId);

            if (neighborhood is not null)
            {
                var campaigns = await _campaignQueries.GetAllByTerritoryIdAsync(TerritoryRef.From(neighborhood.TerritoryId.Value), cancellationToken);

                foreach (var campaign in campaigns)
                {
                    await _writer.WriteAsync(new CampaignValidationSignal(CampaignRef.From(campaign.Id.Value)));
                }
            }
        }

    }
}

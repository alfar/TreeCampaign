using Common.Domain.Abstractions;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams.Events;

public sealed record TeamNameUpdated(TeamId Id, Guid CampaignId, TeamName Name) : IDomainEvent, ICampaignScoped
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}

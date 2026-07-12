using Common.Domain.Abstractions;
using TreeCampaign.Domain.Abstractions;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams.Events;

public sealed record TeamCreated(TeamId Id, CampaignId CampaignId, TeamName Name, TeamKind Kind) : IDomainEvent, ITreeCampaignEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}

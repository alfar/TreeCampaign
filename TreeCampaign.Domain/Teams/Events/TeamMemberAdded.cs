using Common.Domain.Abstractions;
using TreeCampaign.Domain.Abstractions;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams.Events;

public sealed record TeamMemberAdded(TeamId Id, CampaignId CampaignId, string Name, string? ScoutRelativeName, string? PhoneNumber) : IDomainEvent, ITreeCampaignEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}

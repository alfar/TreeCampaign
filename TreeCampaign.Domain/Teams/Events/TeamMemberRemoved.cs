using Common.Domain.Abstractions;
using TreeCampaign.Domain.Abstractions;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.TeamMembers.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams.Events;

public sealed record TeamMemberRemoved(TeamId Id, CampaignId CampaignId, TeamMemberId MemberId) : IDomainEvent, ITreeCampaignEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}

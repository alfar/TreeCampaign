using Common.Domain.Abstractions;
using TreeCampaign.Domain.TeamMembers.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams.Events;

public sealed record TeamMemberRemoved(TeamId Id, Guid CampaignId, TeamMemberId MemberId) : IDomainEvent, ICampaignScoped
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}

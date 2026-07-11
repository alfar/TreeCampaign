using Common.Domain.Abstractions;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams.Events;

public sealed record TeamMemberAdded(TeamId Id, Guid CampaignId, string Name, string? ScoutRelativeName, string PhoneNumber) : IDomainEvent, ICampaignScoped
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}

using Common.Domain.Abstractions;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Stops.Events;

public sealed record StopAssigned(StopId Id, TeamId AssignedTeamId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}

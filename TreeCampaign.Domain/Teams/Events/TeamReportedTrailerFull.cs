using Common.Domain.Abstractions;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams.Events;

public sealed record TeamReportedTrailerFull(TeamId Id) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public Guid AggregateId => Id.Value;
}

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
    Guid AggregateId { get; }
}

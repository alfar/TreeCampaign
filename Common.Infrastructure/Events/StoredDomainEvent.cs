namespace Common.InfraStructure.Events;

public class StoredDomainEvent
{
    public long Id { get; set; }
    public Guid AggregateId { get; set; }
    public string Type { get; set; } = default!;
    public DateTimeOffset OccurredAtUtc { get; set; }
    public string Data { get; set; } = default!;
    public DateTimeOffset? ProcessedAtUtc { get; set; }
}

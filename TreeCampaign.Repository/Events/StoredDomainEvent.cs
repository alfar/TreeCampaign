namespace TreeCampaign.Repository.Events;

public class StoredDomainEvent
{
    public long Id { get; set; }
    public Guid AggregateId { get; set; }
    public string Type { get; set; } = default!;
    public DateTime OccurredAtUtc { get; set; }
    public string Data { get; set; } = default!;
}

namespace Intake.Repository.Queries;

public interface IOrderQueries
{
    Task<IReadOnlyCollection<OrderProjection>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<OrderProjection>> GetByStateAsync(string state, CancellationToken ct = default);
    Task<OrderProjection?> GetByIdAsync(Guid id, CancellationToken ct = default);
}

public class OrderProjection
{
    public Guid Id { get; init; }
    public string OrderType { get; init; } = default!;
    public Guid CampaignId { get; init; }
    public string SenderName { get; init; } = default!;
    public string SenderPhoneNumber { get; init; } = default!;
    public decimal Amount { get; init; }
    public DateTimeOffset OrderDate { get; init; }
    public string Message { get; init; } = default!;
    // WashedOrder only
    public string? WashedStreet { get; init; }
    public string? WashedHouseNumber { get; init; }
    public string? WashedZipCode { get; init; }
    // OutOfBoundsOrder + ValidatedOrder
    public Guid? StreetId { get; init; }
    // ValidatedOrder only
    public Guid? StreetSectionId { get; init; }
    public Guid? NeighborhoodId { get; init; }
}

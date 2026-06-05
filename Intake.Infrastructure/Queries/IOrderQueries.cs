using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.InfraStructure.Queries;

public interface IOrderQueries
{
    Task<IReadOnlyCollection<OrderProjection>> GetAllAsync(CampaignRef campaignId, CancellationToken ct = default);
    Task<IReadOnlyCollection<OrderProjection>> GetByStateAsync(CampaignRef campaignId, string state, CancellationToken ct = default);
    Task<OrderProjection?> GetByIdAsync(OrderId id, CancellationToken ct = default);
}

public class OrderProjection
{
    public required OrderId Id { get; init; }
    public required string OrderType { get; init; }
    public required CampaignRef CampaignId { get; init; }
    public required string SenderName { get; init; }
    public required string SenderPhoneNumber { get; init; }
    public required MoneyAmount Amount { get; init; }
    public required DateTimeOffset OrderDate { get; init; }
    public string Message { get; init; } = default!;
    // OutOfBoundsOrder + ValidatedOrder + WashedOrder
    public StreetRef? StreetId { get; init; }
    // ValidatedOrder + WashedOrder
    public StreetSectionRef? StreetSectionId { get; init; }
    public NeighborhoodRef? NeighborhoodId { get; init; }
}

using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Intake.InfraStructure.Queries;

public class OrderQueries(IntakeProjectionContext context) : IOrderQueries
{
    public async Task<IReadOnlyCollection<OrderProjection>> GetAllAsync(CampaignRef campaignId, CancellationToken ct = default) =>
        await context.Orders.Where(o => o.CampaignId == campaignId).ToListAsync(ct);

    public async Task<IReadOnlyCollection<OrderProjection>> GetByStateAsync(CampaignRef campaignId, string state, CancellationToken ct = default) =>
        await context.Orders.Where(o => o.CampaignId == campaignId && o.OrderType == state).ToListAsync(ct);

    public async Task<OrderProjection?> GetByIdAsync(OrderId id, CancellationToken ct = default) =>
        await context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
}

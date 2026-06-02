using Microsoft.EntityFrameworkCore;

namespace Intake.Repository.Queries;

public class OrderQueries(IntakeProjectionContext context) : IOrderQueries
{
    public async Task<IReadOnlyCollection<OrderProjection>> GetAllAsync(CancellationToken ct = default) =>
        await context.Orders.ToListAsync(ct);

    public async Task<IReadOnlyCollection<OrderProjection>> GetByStateAsync(string state, CancellationToken ct = default) =>
        await context.Orders.Where(o => o.OrderType == state).ToListAsync(ct);

    public async Task<OrderProjection?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
}

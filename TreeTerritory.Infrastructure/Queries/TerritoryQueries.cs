using Microsoft.EntityFrameworkCore;
using TreeTerritory.Domain.Territories;
using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.Infrastructure.Queries;

public interface ITerritoryQueries
{
    Task<IReadOnlyCollection<Territory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Territory?> GetByIdAsync(TerritoryId territoryId, CancellationToken cancellationToken = default);
}

public class TerritoryQueries : ITerritoryQueries
{
    private readonly TreeTerritoryContext _dbContext;

    public TerritoryQueries(TreeTerritoryContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Territory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return [
            .. _dbContext.Territories
            ];
    }

    public async Task<Territory?> GetByIdAsync(TerritoryId territoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Territories.FirstOrDefaultAsync(t => t.Id == territoryId, cancellationToken);
    }
}
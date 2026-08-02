using Microsoft.EntityFrameworkCore;
using TreeTerritory.Domain.ExternalReferences;
using TreeTerritory.Domain.Territories;
using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.Infrastructure.Queries;

public interface ITerritoryQueries
{
    Task<IReadOnlyCollection<Territory>> GetAllAsync(ScoutGroupRef scoutGroupId, CancellationToken cancellationToken = default);
    Task<Territory?> GetByIdAsync(TerritoryId territoryId, CancellationToken cancellationToken = default);
}

public class TerritoryQueries : ITerritoryQueries
{
    private readonly TreeTerritoryContext _dbContext;

    public TerritoryQueries(TreeTerritoryContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Territory>> GetAllAsync(ScoutGroupRef scoutGroupId, CancellationToken cancellationToken = default)
    {
        return [
            .. _dbContext.Territories.AsNoTracking().Where(t => t.ScoutGroupId == scoutGroupId)
            ];
    }

    public async Task<Territory?> GetByIdAsync(TerritoryId territoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Territories.AsNoTracking().FirstOrDefaultAsync(t => t.Id == territoryId, cancellationToken);
    }
}
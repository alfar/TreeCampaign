using TreeTerritory.Domain.Territories;
using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.InfraStructure.Queries;

public interface ITerritoryQueries
{
    Task<IReadOnlyCollection<Territory>> GetAllAsync(CancellationToken cancellationToken = default);
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
}
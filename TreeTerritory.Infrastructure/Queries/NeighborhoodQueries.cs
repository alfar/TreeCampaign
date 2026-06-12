using Microsoft.EntityFrameworkCore;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.Infrastructure.Queries;

public interface INeighborhoodQueries
{
    Task<IReadOnlyCollection<Neighborhood>> GetAllByTerritoryIdAsync(TerritoryId territoryId, CancellationToken cancellationToken = default);
    Task<Neighborhood?> GetByIdAsync(NeighborhoodId neighborhoodId, CancellationToken cancellationToken = default);
}

public class NeighborhoodQueries : INeighborhoodQueries
{
    private readonly TreeTerritoryContext _dbContext;

    public NeighborhoodQueries(TreeTerritoryContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Neighborhood>> GetAllByTerritoryIdAsync(TerritoryId territoryId, CancellationToken cancellationToken = default)
    {
        return await
            _dbContext.Neighborhoods.Include(n => n.StreetSections)
                .Where(n => n.TerritoryId == territoryId)
                .ToListAsync(cancellationToken);
    }

    public async Task<Neighborhood?> GetByIdAsync(NeighborhoodId neighborhoodId, CancellationToken cancellationToken = default)
    {
        return await
            _dbContext.Neighborhoods.Include(n => n.StreetSections)
                .Where(n => n.Id == neighborhoodId)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
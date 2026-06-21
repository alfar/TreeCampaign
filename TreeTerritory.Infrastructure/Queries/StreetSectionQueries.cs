using Microsoft.EntityFrameworkCore;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.StreetSections.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.Infrastructure.Queries;

public interface IStreetSectionQueries
{
    Task<IReadOnlyCollection<StreetSection>> GetByTerritoryAndStreetAsync(
        TerritoryId territoryId, StreetId streetId, CancellationToken cancellationToken = default);
}

public class StreetSectionQueries(TreeTerritoryContext dbContext) : IStreetSectionQueries
{
    public async Task<IReadOnlyCollection<StreetSection>> GetByTerritoryAndStreetAsync(
        TerritoryId territoryId, StreetId streetId, CancellationToken cancellationToken = default)
    {
        return await dbContext.StreetSections
            .Where(ss => ss.StreetId == streetId
                && dbContext.Neighborhoods.Any(n => n.Id == ss.NeighborhoodId && n.TerritoryId == territoryId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<StreetSection>> GetByIdsAsync(IEnumerable<StreetSectionId> ids, CancellationToken cancellationToken)
    {
        return await dbContext.StreetSections.Where(ss => ids.Contains(ss.Id)).ToListAsync(cancellationToken);
    }
}

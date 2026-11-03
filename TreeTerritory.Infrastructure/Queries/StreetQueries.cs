using Microsoft.EntityFrameworkCore;
using TreeTerritory.Domain.Streets;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.Infrastructure.Queries;

public interface IStreetQueries
{
    Task<IEnumerable<Street>> GetByZipCodeAsync(ZipCode zipCode, CancellationToken cancellationToken);
    Task<Street?> GetByNameAndZipCodeAsync(string name, ZipCode zipCode, CancellationToken cancellationToken);
    Task<IEnumerable<Street>> GetByTerritoryAsync(TerritoryId territoryId, CancellationToken cancellationToken);
}

public class StreetQueries : IStreetQueries
{
    private readonly TreeTerritoryContext _context;

    public StreetQueries(TreeTerritoryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Street>> GetByZipCodeAsync(ZipCode zipCode, CancellationToken cancellationToken)
    {
        return await _context.Streets.Where(street => street.ZipCode == zipCode).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Street>> GetByTerritoryAsync(TerritoryId territoryId, CancellationToken cancellationToken)
    {
        return await _context.Streets
            .Where(street => _context.StreetSections.Any(section => section.StreetId == street.Id
                && _context.Neighborhoods.Any(n => n.Id == section.NeighborhoodId && n.TerritoryId == territoryId)))
            .OrderBy(street => street.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Street?> GetByNameAndZipCodeAsync(string name, ZipCode zipCode, CancellationToken cancellationToken)
    {
        return await _context.Streets
            .Where(street => street.ZipCode == zipCode && EF.Functions.Like(street.Name, name))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
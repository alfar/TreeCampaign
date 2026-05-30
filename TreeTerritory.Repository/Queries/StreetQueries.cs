using Microsoft.EntityFrameworkCore;
using TreeTerritory.Domain.Streets;
using TreeTerritory.Domain.Streets.ValueObjects;

namespace TreeTerritory.Repository.Queries;

public interface IStreetQueries
{
    Task<IEnumerable<Street>> GetByZipCodeAsync(ZipCode zipCode, CancellationToken cancellationToken);
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
}
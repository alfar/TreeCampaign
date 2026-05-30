
using Common.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TreeTerritory.Domain.Neighborhoods;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;
using TreeTerritory.Domain.Streets;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections;
using TreeTerritory.Domain.StreetSections.ValueObjects;
using TreeTerritory.Domain.Territories;
using TreeTerritory.Domain.Territories.ValueObjects;
using TreeTerritory.Repository.Configuration;

namespace TreeTerritory.Repository;

public class TreeTerritoryContext(DbContextOptions<TreeTerritoryContext> options)
    : DbContext(options),
        IUnitOfWork,
        IRepository<Territory, TerritoryId>,
        IRepository<Neighborhood, NeighborhoodId>,
        IRepository<Street, StreetId>,
        IRepository<StreetSection, StreetSectionId>
{
    public DbSet<Territory> Territories { get; set; }
    internal DbSet<Neighborhood> Neighborhoods { get; set; }
    public DbSet<Street> Streets { get; set; }
    public DbSet<StreetSection> StreetSections { get; set; }

    public void Add(Territory aggregate) => Territories.Add(aggregate);

    public void Delete(Territory aggregate) => Territories.Remove(aggregate);

    async Task<Territory?> IRepository<Territory, TerritoryId>.TryFindAsync(TerritoryId key, CancellationToken cancellationToken) =>
        await Territories.FirstOrDefaultAsync(t => t.Id == key, cancellationToken);

    public void Add(Neighborhood aggregate) => Neighborhoods.Add(aggregate);

    public void Delete(Neighborhood aggregate) => Neighborhoods.Remove(aggregate);

    async Task<Neighborhood?> IRepository<Neighborhood, NeighborhoodId>.TryFindAsync(NeighborhoodId key, CancellationToken cancellationToken) =>
        await Neighborhoods.Include(n => n.StreetSections).FirstOrDefaultAsync(n => n.Id == key, cancellationToken);

    public void Add(Street aggregate) => Streets.Add(aggregate);

    public void Delete(Street aggregate) => Streets.Remove(aggregate);

    async Task<Street?> IRepository<Street, StreetId>.TryFindAsync(StreetId key, CancellationToken cancellationToken) =>
        await Streets.FirstOrDefaultAsync(s => s.Id == key, cancellationToken);

    async Task<StreetSection?> IRepository<StreetSection, StreetSectionId>.TryFindAsync(StreetSectionId key, CancellationToken cancellationToken) =>
        await StreetSections.FirstOrDefaultAsync(ss => ss.Id == key, cancellationToken);

    public void Add(StreetSection aggregate) => StreetSections.Add(aggregate);

    public void Delete(StreetSection aggregate) => StreetSections.Remove(aggregate);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddTerritories().AddNeighborhoods().AddStreets().AddStreetSections();
    }

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() =>
        (IRepository<TAggregate, TKey>)this;
}

public class TreeTerritoryContextFactory : IDesignTimeDbContextFactory<TreeTerritoryContext>
{
    public TreeTerritoryContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TreeTerritoryContext>();
        var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new TreeTerritoryContext(optionsBuilder.Options);
    }
}

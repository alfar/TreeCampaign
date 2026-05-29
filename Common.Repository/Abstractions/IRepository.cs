using System;

namespace Common.Repository.Abstractions;

public interface IRepository<TAggregate, TKey>
{
    Task<TAggregate?> TryFindAsync(TKey key);
    void Add(TAggregate aggregate);
    void Delete(TAggregate aggregate);

    public async Task Delete(TKey key)
    {
        var aggregate =
            await TryFindAsync(key)
            ?? throw new ArgumentException($"Aggregate with key {key} not found.");

        Delete(aggregate);
    }
}

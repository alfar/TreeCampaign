using System;

namespace Common.InfraStructure.Abstractions;

public interface IUnitOfWork
{
    IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

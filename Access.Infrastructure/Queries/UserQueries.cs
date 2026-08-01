using Access.Domain.ScoutGroups.ValueObjects;
using Access.Domain.Users;
using Access.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Access.Infrastructure.Queries;

public interface IUserQueries
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<User>> GetByScoutGroupIdAsync(ScoutGroupId scoutGroupId, CancellationToken cancellationToken = default);
}

public class UserQueries : IUserQueries
{
    private readonly AccessContext _dbContext;

    public UserQueries(AccessContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetByScoutGroupIdAsync(ScoutGroupId scoutGroupId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.Where(u => u.ScoutGroupId == scoutGroupId).ToListAsync(cancellationToken);
    }
}

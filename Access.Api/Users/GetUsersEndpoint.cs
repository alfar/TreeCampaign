using Access.Domain.ScoutGroups.ValueObjects;
using Access.Infrastructure.Queries;

namespace Access.Api.Users;

internal class GetUsersEndpoint
{
    internal static async Task<IResult> Handle(
        ScoutGroupId scoutGroupId,
        IUserQueries userQueries,
        CancellationToken cancellationToken
    )
    {
        var users = await userQueries.GetByScoutGroupIdAsync(scoutGroupId, cancellationToken);
        return Results.Ok(users);
    }
}

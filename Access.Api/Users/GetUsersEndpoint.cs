using Access.Domain.ScoutGroups.ValueObjects;
using Access.Infrastructure.Queries;
using Access.Api.Users.Dto;
using Common.Infrastructure.Auth;
using Access.Api.Helpers;

namespace Access.Api.Users;

internal class GetUsersEndpoint
{
    internal static async Task<IResult> Handle(
        ICurrentUserAccessor userAccessor,
        IUserQueries userQueries,
        CancellationToken cancellationToken
    )
    {
        var users = await userQueries.GetByScoutGroupIdAsync(userAccessor.GetScoutGroupId(), cancellationToken);
        return Results.Ok(users.Select(u => UserDto.From(u)));
    }
}

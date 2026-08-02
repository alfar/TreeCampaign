using Common.Infrastructure.Auth;
using Access.Domain.ScoutGroups.ValueObjects;

namespace Access.Api.Helpers;

internal static class CurrentUserAccessorExtensions
{
    internal static ScoutGroupId GetScoutGroupId(this ICurrentUserAccessor currentUser) =>
        ScoutGroupId.From(currentUser.ScoutGroupId!.Value);
}

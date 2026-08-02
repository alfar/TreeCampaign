using Common.Infrastructure.Auth;
using TreeTerritory.Domain.ExternalReferences;

namespace TreeTerritory.Api.Helpers;

internal static class CurrentUserAccessorExtensions
{
    internal static ScoutGroupRef GetScoutGroupId(this ICurrentUserAccessor currentUser) =>
        ScoutGroupRef.From(currentUser.ScoutGroupId!.Value);
}

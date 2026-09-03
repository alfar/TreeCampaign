using Common.Infrastructure.Auth;
using Intake.Domain.ExternalReferences;

namespace Intake.Api.Helpers;

internal static class CurrentUserAccessorExtensions
{
    internal static ScoutGroupRef GetScoutGroupId(this ICurrentUserAccessor currentUser) =>
        ScoutGroupRef.From(currentUser.ScoutGroupId!.Value);
}

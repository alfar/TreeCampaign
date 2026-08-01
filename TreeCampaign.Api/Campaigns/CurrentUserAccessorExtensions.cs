using Common.Infrastructure.Auth;
using TreeCampaign.Domain.ExternalReferences;

namespace TreeCampaign.Api.Campaigns;

internal static class CurrentUserAccessorExtensions
{
    internal static ScoutGroupRef GetScoutGroupId(this ICurrentUserAccessor currentUser) =>
        ScoutGroupRef.From(currentUser.ScoutGroupId!.Value);
}

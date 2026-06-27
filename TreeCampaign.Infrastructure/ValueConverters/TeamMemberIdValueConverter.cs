using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.TeamMembers.ValueObjects;

namespace TreeCampaign.Infrastructure.ValueConverters;

public class TeamMemberIdValueConverter : ValueConverter<TeamMemberId, Guid>
{
    public TeamMemberIdValueConverter()
        : base(id => id.Value, value => TeamMemberId.From(value)) { }
}

public class NullableTeamMemberIdValueConverter : ValueConverter<TeamMemberId?, Guid?>
{
    public NullableTeamMemberIdValueConverter()
        : base(
            id => id != null ? id.Value : null,
            value => value.HasValue ? TeamMemberId.From(value.Value) : null
        ) { }
}

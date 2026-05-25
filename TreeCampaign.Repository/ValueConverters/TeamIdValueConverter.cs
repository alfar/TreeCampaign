using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Repository.ValueConverters;

public class TeamIdValueConverter : ValueConverter<TeamId, Guid>
{
    public TeamIdValueConverter()
        : base(id => id.Value, value => TeamId.From(value)) { }
}

public class NullableTeamIdValueConverter : ValueConverter<TeamId?, Guid?>
{
    public NullableTeamIdValueConverter()
        : base(
            id => id != null ? id.Value : null,
            value => value.HasValue ? TeamId.From(value.Value) : null
        ) { }
}

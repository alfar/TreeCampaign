using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.Teams.ValueObjects;

internal class TeamNameValueConverter : ValueConverter<TeamName, string>
{
    public TeamNameValueConverter()
        : base(teamName => teamName.Value, value => TeamName.From(value)) { }
}

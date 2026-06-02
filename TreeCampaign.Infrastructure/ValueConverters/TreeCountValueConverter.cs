using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.Stops;

internal class TreeCountValueConverter : ValueConverter<TreeCount, int>
{
    public TreeCountValueConverter()
        : base(count => count.Value, value => TreeCount.From(value)) { }
}

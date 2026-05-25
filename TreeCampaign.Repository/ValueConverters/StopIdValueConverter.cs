using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.Stops;

namespace TreeCampaign.Repository.ValueConverters;

public class StopIdValueConverter : ValueConverter<StopId, Guid>
{
    public StopIdValueConverter()
        : base(id => id.Value, value => StopId.From(value)) { }
}

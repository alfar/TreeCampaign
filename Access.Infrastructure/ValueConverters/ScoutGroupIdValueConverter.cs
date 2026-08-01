using Access.Domain.ScoutGroups.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Access.Infrastructure.ValueConverters;

internal class ScoutGroupIdValueConverter : ValueConverter<ScoutGroupId, Guid>
{
    public ScoutGroupIdValueConverter()
        : base(id => id.Value, value => ScoutGroupId.From(value)) { }
}

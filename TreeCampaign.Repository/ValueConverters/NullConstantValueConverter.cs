using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace TreeCampaign.Repository.ValueConverters;

public class NullValueGenerator<T> : ValueGenerator<T?>
    where T : struct
{
    public override bool GeneratesTemporaryValues => false;

    public override T? Next(EntityEntry entry)
    {
        return null;
    }
}

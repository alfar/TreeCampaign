using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Infrastructure.ValueConverters;

internal class NullableMoneyAmountValueConverter : ValueConverter<MoneyAmount?, decimal?>
{
    public NullableMoneyAmountValueConverter()
        : base(money => money != null ? money.Value : null, value => value.HasValue ? new MoneyAmount(value.Value) : null) { }
}

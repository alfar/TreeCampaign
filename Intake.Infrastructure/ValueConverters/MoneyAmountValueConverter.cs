using Intake.Domain.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.Infrastructure.ValueConverters;

internal class MoneyAmountValueConverter : ValueConverter<MoneyAmount, decimal>
{
    public MoneyAmountValueConverter() : base(m => m.Value, value => new MoneyAmount(value)) { }
}

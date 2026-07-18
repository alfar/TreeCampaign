using Intake.Domain.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.Infrastructure.ValueConverters;

internal class TransactionIdValueConverter : ValueConverter<TransactionId?, string?>
{
    public TransactionIdValueConverter() : base(id => id == null ? null : id.Value, value => value == null ? null : new TransactionId(value)) { }
}

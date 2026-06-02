using Intake.Domain.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.Repository.ValueConverters;

internal class OrderIdValueConverter : ValueConverter<OrderId, Guid>
{
    public OrderIdValueConverter() : base(id => id.Value, value => new OrderId(value)) { }
}

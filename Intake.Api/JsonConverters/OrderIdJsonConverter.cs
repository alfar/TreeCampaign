using System.Text.Json;
using System.Text.Json.Serialization;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Api.JsonConverters;

public class OrderIdJsonConverter : JsonConverter<OrderId>
{
    public override OrderId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return OrderId.From(value);
    }

    public override void Write(Utf8JsonWriter writer, OrderId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using Intake.Domain.Orders.ValueObjects;

public class MoneyAmountJsonConverter : JsonConverter<MoneyAmount>
{
    public override MoneyAmount Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetDecimal();
        return MoneyAmount.From(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        MoneyAmount value,
        JsonSerializerOptions options
    )
    {
        writer.WriteNumberValue(value.Value);
    }
}

using Intake.Domain.Orders.ValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Intake.Api.JsonConverters;

public class HouseNumberJsonConverter : JsonConverter<HouseNumber>
{
    public override HouseNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString() ?? string.Empty;
        return HouseNumber.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, HouseNumber value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}


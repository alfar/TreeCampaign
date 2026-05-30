using System.Text.Json;
using System.Text.Json.Serialization;
using TreeTerritory.Domain.Streets.ValueObjects;

namespace TreeTerritory.Api.JsonConverters;

public class ZipCodeJsonConverter : JsonConverter<ZipCode>
{
    public override ZipCode Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetString();
        return value is null ? ZipCode.Empty : ZipCode.From(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        ZipCode value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}

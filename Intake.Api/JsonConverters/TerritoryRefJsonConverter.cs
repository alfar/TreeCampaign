using System.Text.Json;
using System.Text.Json.Serialization;
using Intake.Domain.ExternalReferences;

namespace Intake.Api.JsonConverters;

public class TerritoryRefJsonConverter : JsonConverter<TerritoryRef>
{
    public override TerritoryRef Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return TerritoryRef.From(value);
    }

    public override void Write(Utf8JsonWriter writer, TerritoryRef value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}

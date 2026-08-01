using System.Text.Json;
using System.Text.Json.Serialization;
using Access.Domain.Users.ValueObjects;

namespace Access.Api.JsonConverters;

public class UserIdJsonConverter : JsonConverter<UserId>
{
    public override UserId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetGuid();
        return UserId.From(value);
    }

    public override void Write(Utf8JsonWriter writer, UserId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}

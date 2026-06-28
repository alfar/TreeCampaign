using System.Text.Json;

namespace Common.Infrastructure.Services;

public class SseJsonOptions
{
    public JsonSerializerOptions SerializerOptions { get; } = new(JsonSerializerDefaults.Web);
}

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure;

// value objects as one json column; the comparer compares the json, because a record with a list compares the list
// by reference, and the tracker snapshot is a json copy
internal static class JsonColumnExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> builder)
    {
        return builder.HasConversion(
            value => ToJson(value),
            json => FromJson<T>(json),
            new ValueComparer<T>(
                (left, right) => ToJson(left) == ToJson(right),
                value => ToJson(value).GetHashCode(),
                value => FromJson<T>(ToJson(value))));
    }

    private static string ToJson<T>(T value)
    {
        return JsonSerializer.Serialize(value, JsonOptions);
    }

    private static T FromJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, JsonOptions)!;
    }
}

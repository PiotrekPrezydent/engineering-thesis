using System.Text.Json;
using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Plugins.Infrastructure.Plugins;

public class PluginFunctionEntityTypeConfiguration : IEntityTypeConfiguration<PluginFunction>
{
    // the description is one json document per column, so a richer type model needs no table change
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public void Configure(EntityTypeBuilder<PluginFunction> builder)
    {
        builder.ToTable(PluginFunction.DbTableName);
        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value, 
                value => new PluginFunctionId(value));
        
        builder.Property(p => p.Parameters)
            .HasConversion(
                parameters => ToJson(parameters),
                json => FromJson<IReadOnlyList<PropertyDescription>>(json)!,
                JsonComparer<IReadOnlyList<PropertyDescription>>());

        builder.Property(p => p.Returns)
            .HasConversion(
                returns => ToJson(returns),
                json => FromJson<TypeDescription>(json),
                JsonComparer<TypeDescription?>());
    }

    private static string ToJson<T>(T value)
    {
        return JsonSerializer.Serialize(value, JsonOptions);
    }

    private static T? FromJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    // descriptions never change after create, so the snapshot can keep the same instance
    private static ValueComparer<T> JsonComparer<T>()
    {
        return new ValueComparer<T>(
            (left, right) => ToJson(left) == ToJson(right),
            value => ToJson(value).GetHashCode(),
            value => value);
    }
}
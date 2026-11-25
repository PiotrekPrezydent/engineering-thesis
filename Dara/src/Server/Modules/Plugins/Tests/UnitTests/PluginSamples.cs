using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;

namespace Dara.Server.Modules.Plugins.UnitTests;

// small valid plugins for the rule tests, each test breaks one thing
internal static class PluginSamples
{
    public static TypeDescription Text => new(TypeKind.String);

    public static TypeDescription Integer => new(TypeKind.Integer);

    public static PluginHash Hash(char digit) => new(new string(digit, 64));

    public static FunctionDescription Function(string name, params PropertyDescription[] parameters)
        => new(name, $"{name} description", parameters, null);

    public static FunctionDescription SaveFile => new("SaveFile", "Saves a file",
    [
        new PropertyDescription("path", Text with { Pattern = "^/" }, true),
        new PropertyDescription("content", Text with { ContentEncoding = "base64" }, true),
        new PropertyDescription("overwrite", new TypeDescription(TypeKind.Boolean, Default: "false"), false)
    ], new TypeDescription(TypeKind.Boolean));

    public static PluginOwner Owner() => PluginOwner.CreateDefault(new PluginOwnerId(Guid.NewGuid()));

    public static Plugin AddSample(this PluginOwner owner, string name = "Files", char hash = 'a',
        IReadOnlyList<FunctionDescription>? functions = null, IReadOnlyList<Plugin>? owned = null)
        => owner.AddPlugin(owned ?? [], name, "Plugin description", "1.0.0", Hash(hash), functions ?? [SaveFile]);
}

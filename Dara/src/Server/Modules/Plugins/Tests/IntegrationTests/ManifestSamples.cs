using System.Text.Json;
using Dara.Shared.Contracts.Functions;
using Dara.Shared.Contracts.Plugins;

namespace Dara.Server.Modules.Plugins.IntegrationTests;

// manifests as a client sends them, the hash digit tells versions of one plugin apart
internal static class ManifestSamples
{
    private static TypeDescription Text => new(TypeKind.String);

    public static string Hash(char digit) => new(digit, 64);

    public static FunctionDescription SaveFile => new("SaveFile", "Saves content as a file",
    [
        new PropertyDescription("path", Text with { Description = "Where the file is saved", Pattern = "^/" }, true),
        new PropertyDescription("content", Text with { ContentEncoding = "base64" }, true),
        new PropertyDescription("overwrite", new TypeDescription(TypeKind.Boolean, Default: "false"), false)
    ], new TypeDescription(TypeKind.Boolean));

    public static FunctionDescription ReadFile => new("ReadFile", "Reads a file",
    [
        new PropertyDescription("path", Text, true)
    ], Text with { Nullable = true });

    public static FunctionDescription TakePhoto => new("TakePhoto", null,
    [
        new PropertyDescription("size", new TypeDescription(TypeKind.Object, Properties:
        [
            new PropertyDescription("width", new TypeDescription(TypeKind.Integer, Minimum: 1), true),
            new PropertyDescription("height", new TypeDescription(TypeKind.Integer, Minimum: 1), true)
        ]), false)
    ], new TypeDescription(TypeKind.Array, Items: Text with { Format = "uri" }));

    public static PluginManifest Files(char hash = 'a') => new("Files", "File management", "1.0.0", Hash(hash), [SaveFile, ReadFile]);

    public static PluginManifest Camera(char hash = 'c') => new("Camera", "Device camera", "2.0.0", Hash(hash), [TakePhoto]);

    public static string Json(FunctionDescription description) => JsonSerializer.Serialize(description);
}

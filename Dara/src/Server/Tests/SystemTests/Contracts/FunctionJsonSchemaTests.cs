using System.Text.Json;
using System.Text.Json.Nodes;
using Dara.Shared.Contracts.Functions;
using Dara.Shared.Contracts.Plugins;

namespace Dara.Server.SystemTests.Contracts;

public class FunctionJsonSchemaTests
{
    private static TypeDescription Text => new(TypeKind.String);

    private static TypeDescription Integer => new(TypeKind.Integer);

    // one field of the description and the schema it must give
    private static readonly Dictionary<string, (TypeDescription Type, string Schema)> Keywords = new()
    {
        ["kind string"] = (Text, """{"type":"string"}"""),
        ["kind integer"] = (Integer, """{"type":"integer"}"""),
        ["kind number"] = (new TypeDescription(TypeKind.Number), """{"type":"number"}"""),
        ["kind boolean"] = (new TypeDescription(TypeKind.Boolean), """{"type":"boolean"}"""),
        ["kind object"] = (new TypeDescription(TypeKind.Object), """{"type":"object"}"""),
        ["kind array"] = (new TypeDescription(TypeKind.Array), """{"type":"array"}"""),
        ["kind any"] = (new TypeDescription(TypeKind.Any), "true"),
        ["any with description"] = (new TypeDescription(TypeKind.Any, Description: "x"), """{"description":"x"}"""),
        ["nullable"] = (Text with { Nullable = true }, """{"type":["string","null"]}"""),
        ["description"] = (Text with { Description = "a text" }, """{"description":"a text","type":"string"}"""),
        ["format"] = (Text with { Format = "uuid" }, """{"type":"string","format":"uuid"}"""),
        ["pattern"] = (Text with { Pattern = "^[a-z]+$" }, """{"type":"string","pattern":"^[a-z]+$"}"""),
        ["min length"] = (Text with { MinLength = 2 }, """{"type":"string","minLength":2}"""),
        ["max length"] = (Text with { MaxLength = 20 }, """{"type":"string","maxLength":20}"""),
        ["minimum"] = (Integer with { Minimum = 1 }, """{"type":"integer","minimum":1}"""),
        ["maximum"] = (new TypeDescription(TypeKind.Number, Maximum: 9.5m), """{"type":"number","maximum":9.5}"""),
        ["min items"] = (new TypeDescription(TypeKind.Array, MinItems: 1), """{"type":"array","minItems":1}"""),
        ["max items"] = (new TypeDescription(TypeKind.Array, MaxItems: 5), """{"type":"array","maxItems":5}"""),
        ["enum"] = (Text with { Enum = ["Red", "Green"] }, """{"type":"string","enum":["Red","Green"]}"""),
        ["nullable enum"] = (Text with { Nullable = true, Enum = ["Red"] }, """{"type":["string","null"],"enum":["Red",null]}"""),
        ["default"] = (Integer with { Default = "5" }, """{"type":"integer","default":5}"""),
        ["null default"] = (Text with { Nullable = true, Default = "null" }, """{"type":["string","null"],"default":null}"""),
        ["object default"] = (new TypeDescription(TypeKind.Object, Default: """{"a":1}"""), """{"type":"object","default":{"a":1}}"""),
        ["content encoding"] = (Text with { ContentEncoding = "base64" }, """{"type":"string","contentEncoding":"base64"}"""),
        ["items"] = (new TypeDescription(TypeKind.Array, Items: Integer), """{"type":"array","items":{"type":"integer"}}"""),
        ["nested items"] = (new TypeDescription(TypeKind.Array, Items: new TypeDescription(TypeKind.Array, Items: Integer)),
            """{"type":"array","items":{"type":"array","items":{"type":"integer"}}}"""),
        ["properties"] = (new TypeDescription(TypeKind.Object, Properties: [new("name", Text, true), new("age", Integer, false)]),
            """{"type":"object","properties":{"name":{"type":"string"},"age":{"type":"integer"}},"required":["name"]}"""),
        ["properties none required"] = (new TypeDescription(TypeKind.Object, Properties: [new("age", Integer, false)]),
            """{"type":"object","properties":{"age":{"type":"integer"}}}"""),
        ["empty properties"] = (new TypeDescription(TypeKind.Object, Properties: []), """{"type":"object","properties":{}}"""),
        ["additional properties"] = (new TypeDescription(TypeKind.Object, AdditionalProperties: Integer),
            """{"type":"object","additionalProperties":{"type":"integer"}}""")
    };

    public static TheoryData<string> KeywordNames => new(Keywords.Keys);

    [Theory]
    [MemberData(nameof(KeywordNames))]
    public void EveryFieldGivesItsKeyword(string name)
    {
        var (type, schema) = Keywords[name];

        AssertSameJson(schema, FunctionJsonSchema.CreateNode(type));
    }

    [Fact]
    public void NullFieldsDoNotReachTheSchema()
    {
        var node = FunctionJsonSchema.CreateNode(Text);

        var keywords = node.AsObject().Select(property => property.Key);
        Assert.Equal(new[] { "type" }, keywords);
    }

    [Fact]
    public void FunctionWithoutParametersHasAnEmptyObjectSchema()
    {
        var function = new FunctionDescription("Ping", null, [], null);

        AssertSameJson("""{"type":"object","properties":{}}""", FunctionJsonSchema.CreateParametersSchema(function));
        Assert.Null(FunctionJsonSchema.CreateReturnSchema(function));
    }

    [Fact]
    public void ParametersGiveRequiredInTheirOrder()
    {
        var function = new FunctionDescription("Echo", "Repeats a text",
        [
            new("text", Text, true),
            new("count", Integer with { Default = "5" }, false),
            new("suffix", Text with { Nullable = true }, true)
        ], Text);

        AssertSameJson(
            """{"type":"object","properties":{"text":{"type":"string"},"count":{"type":"integer","default":5},"suffix":{"type":["string","null"]}},"required":["text","suffix"]}""",
            FunctionJsonSchema.CreateParametersSchema(function));
        AssertSameJson("""{"type":"string"}""", FunctionJsonSchema.CreateReturnSchema(function)!.Value);
    }

    [Fact]
    public void ManifestSurvivesJsonLikeOverSignalR()
    {
        var manifest = new PluginManifest("Files", null, "1.0.0", new string('a', 64),
        [
            new FunctionDescription("SaveFile", "Saves a file",
            [
                new("path", Text with { Pattern = "^/" }, true),
                new("tags", new TypeDescription(TypeKind.Array, Items: Text with { Enum = ["a", "b"] }), false),
                new("overwrite", new TypeDescription(TypeKind.Boolean, Default: "false"), false)
            ], new TypeDescription(TypeKind.Any))
        ]);

        var json = JsonSerializer.Serialize(manifest, JsonSerializerOptions.Web);
        var copy = JsonSerializer.Deserialize<PluginManifest>(json, JsonSerializerOptions.Web)!;

        Assert.Contains("\"kind\":\"String\"", json);
        var function = Assert.Single(copy.Functions);
        Assert.True(JsonElement.DeepEquals(
            FunctionJsonSchema.CreateParametersSchema(manifest.Functions[0]),
            FunctionJsonSchema.CreateParametersSchema(function)));
        Assert.True(JsonElement.DeepEquals(
            FunctionJsonSchema.CreateReturnSchema(manifest.Functions[0])!.Value,
            FunctionJsonSchema.CreateReturnSchema(function)!.Value));
    }

    private static void AssertSameJson(string expected, JsonNode actual)
    {
        Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), actual), $"expected {expected}, got {actual.ToJsonString()}");
    }

    private static void AssertSameJson(string expected, JsonElement actual)
    {
        using var document = JsonDocument.Parse(expected);
        Assert.True(JsonElement.DeepEquals(document.RootElement, actual), $"expected {expected}, got {actual.GetRawText()}");
    }
}

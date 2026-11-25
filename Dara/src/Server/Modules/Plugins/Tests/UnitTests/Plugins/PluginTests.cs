using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;
using Dara.Server.Modules.Plugins.Domain.Plugins.Events;
using static Dara.Server.Modules.Plugins.UnitTests.PluginSamples;

namespace Dara.Server.Modules.Plugins.UnitTests.Plugins;

public class PluginTests
{
    [Fact]
    public void CreatedPluginKeepsTheFullDescriptionAndRaisesAddedEvent()
    {
        var owner = Owner();

        var plugin = owner.AddSample(functions: [SaveFile, Function("Ping")]);

        Assert.Equal(owner.Id, plugin.OwnerId);
        Assert.Equal(new[] { "SaveFile", "Ping" }, plugin.Functions.Select(f => f.Name));
        var saveFile = plugin.Functions[0].GetDescription();
        Assert.Equal(SaveFile.Parameters, saveFile.Parameters);
        Assert.Equal(SaveFile.Returns, saveFile.Returns);

        var added = Assert.IsType<PluginAddedDomainEvent>(Assert.Single(plugin.DomainEvents));
        Assert.Equal(plugin.Id, added.PluginId);
        Assert.Equal(plugin.Functions.Select(f => f.Id), added.FunctionIds);
    }

    [Fact]
    public void OptionalParameterKeepsItsDefault()
    {
        var plugin = Owner().AddSample();

        var overwrite = plugin.Functions[0].Parameters.Single(p => p.Name == "overwrite");

        Assert.False(overwrite.Required);
        Assert.Equal("false", overwrite.Type.Default);
    }

    [Fact]
    public void RichDescriptionIsValid()
    {
        var address = new TypeDescription(TypeKind.Object, Nullable: true, Properties:
        [
            new("street", Text, true),
            new("zip", Text with { Nullable = true, MaxLength = 6 }, false)
        ]);
        var function = new FunctionDescription("Find", null,
        [
            new("id", Text with { Format = "uuid" }, true),
            new("color", Text with { Nullable = true, Enum = ["Red", "Green"] }, true),
            new("level", Integer with { Minimum = 1, Maximum = 10, Default = "5" }, false),
            new("matrix", new TypeDescription(TypeKind.Array, MinItems: 1, Items: new TypeDescription(TypeKind.Array, Items: Integer)), true),
            new("counts", new TypeDescription(TypeKind.Object, AdditionalProperties: Integer), true),
            new("home", address, true),
            new("extra", new TypeDescription(TypeKind.Any, Description: "anything", Default: "null"), false)
        ], address);

        var plugin = Owner().AddSample(functions: [function]);

        Assert.Single(plugin.Functions);
    }

    [Fact]
    public void PluginNameMustBeProvided()
    {
        AssertBroken<PluginNameMustBeProvidedRule>(() => Owner().AddSample(name: " "));
    }

    [Fact]
    public void FunctionNamesMustBeUniqueIgnoringCase()
    {
        Owner().AddSample(functions: [Function("Save"), Function("Load")]);

        AssertBroken<FunctionNamesMustBeUniqueRule>(() => Owner().AddSample(functions: [Function("Save"), Function("save")]));
    }

    [Fact]
    public void ParameterNamesMustBeUnique()
    {
        Owner().AddSample(functions: [Function("Save", new PropertyDescription("path", Text, true), new PropertyDescription("Path", Text, true))]);

        AssertBroken<PropertyNamesMustBeUniqueRule>(() =>
            Owner().AddSample(functions: [Function("Save", new PropertyDescription("path", Text, true), new PropertyDescription("path", Integer, true))]));
    }

    [Fact]
    public void PropertyNamesMustBeUniqueInNestedObjects()
    {
        var nested = new TypeDescription(TypeKind.Object, Properties: [new("a", Text, true), new("a", Text, false)]);
        var array = new TypeDescription(TypeKind.Array, Items: nested);

        AssertBroken<PropertyNamesMustBeUniqueRule>(() => Owner().AddSample(functions: [Function("Save", new PropertyDescription("items", array, true))]));
    }

    [Theory]
    [InlineData("Save_File")]
    [InlineData("_save")]
    [InlineData("s")]
    public void AllowedFunctionNamesFitProviders(string name)
    {
        Owner().AddSample(functions: [Function(name)]);
    }

    [Theory]
    [InlineData("Save File")]
    [InlineData("1save")]
    [InlineData("save-file")]
    [InlineData("save.file")]
    [InlineData("zapisz_plik_ąę")]
    [InlineData("")]
    public void FunctionNamesOutsideProviderLimitsAreRejected(string name)
    {
        AssertBroken<NamesMustFitProviderLimitsRule>(() => Owner().AddSample(functions: [Function(name)]));
    }

    [Fact]
    public void NamesAreLimitedTo64Characters()
    {
        Owner().AddSample(functions: [Function(new string('a', 64))]);

        AssertBroken<NamesMustFitProviderLimitsRule>(() => Owner().AddSample(functions: [Function(new string('a', 65))]));
    }

    [Fact]
    public void NestedPropertyNamesFitProviderLimits()
    {
        var nested = new TypeDescription(TypeKind.Object, Properties: [new("zip-code", Text, true)]);

        AssertBroken<NamesMustFitProviderLimitsRule>(() => Owner().AddSample(functions: [Function("Save", new PropertyDescription("address", nested, true))]));
    }

    // one broken thing per case, the rest of the description is valid
    private static readonly Dictionary<string, PropertyDescription> InvalidParameters = new()
    {
        ["array without items"] = new("p", new TypeDescription(TypeKind.Array), true),
        ["items on a string"] = new("p", Text with { Items = Text }, true),
        ["enum on an integer"] = new("p", Integer with { Enum = ["1"] }, true),
        ["empty enum"] = new("p", Text with { Enum = [] }, true),
        ["repeated enum value"] = new("p", Text with { Enum = ["a", "a"] }, true),
        ["minimum on a string"] = new("p", Text with { Minimum = 1 }, true),
        ["length on an integer"] = new("p", Integer with { MaxLength = 3 }, true),
        ["format on an integer"] = new("p", Integer with { Format = "uuid" }, true),
        ["properties on a string"] = new("p", Text with { Properties = [] }, true),
        ["item limits on an object"] = new("p", new TypeDescription(TypeKind.Object, MinItems: 1), true),
        ["minimum above maximum"] = new("p", Integer with { Minimum = 10, Maximum = 1 }, true),
        ["min length above max length"] = new("p", Text with { MinLength = 5, MaxLength = 2 }, true),
        ["negative limit"] = new("p", Text with { MinLength = -1 }, true),
        ["pattern is not a regex"] = new("p", Text with { Pattern = "([" }, true),
        ["default is not json"] = new("p", Text with { Default = "{" }, false),
        ["required with a default"] = new("p", Integer with { Default = "5" }, true),
        ["nullable any"] = new("p", new TypeDescription(TypeKind.Any, Nullable: true), true),
        ["unknown kind"] = new("p", new TypeDescription((TypeKind)42), true),
        ["invalid nested item"] = new("p", new TypeDescription(TypeKind.Array, Items: new TypeDescription(TypeKind.Array)), true),
        ["invalid dictionary value"] = new("p", new TypeDescription(TypeKind.Object, AdditionalProperties: Integer with { Pattern = "a" }), true)
    };

    public static TheoryData<string> InvalidParameterNames => new(InvalidParameters.Keys);

    [Theory]
    [MemberData(nameof(InvalidParameterNames))]
    public void InvalidTypeDescriptionIsRejected(string name)
    {
        var function = Function("Save", InvalidParameters[name]);

        AssertBroken<TypeDescriptionMustBeValidRule>(() => Owner().AddSample(functions: [function]));
    }

    [Fact]
    public void InvalidReturnTypeIsRejected()
    {
        var function = Function("Load") with { Returns = new TypeDescription(TypeKind.Array) };

        var exception = AssertBroken<TypeDescriptionMustBeValidRule>(() => Owner().AddSample(functions: [function]));
        Assert.Contains("Load returns", exception.Message);
    }

    [Fact]
    public void HashMustBeSha256AndIsStoredLowercase()
    {
        var owner = Owner();

        var plugin = owner.AddSample(hash: 'A');

        Assert.Equal(new string('a', 64), plugin.Hash.Value);
        AssertBroken<PluginCannotBeAddedTwiceRule>(() => owner.AddSample(name: "Copy", hash: 'a', owned: [plugin]));
        AssertBroken<PluginHashMustBeSha256Rule>(() => Owner().AddPlugin([], "Files", null, "1.0.0", new PluginHash("abc"), [SaveFile]));
        AssertBroken<PluginHashMustBeSha256Rule>(() => Owner().AddSample(hash: 'g'));
    }

    private static BuisnessRuleValidationException AssertBroken<TRule>(Action action)
    {
        var exception = Assert.Throws<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(exception.BrokenRule);
        return exception;
    }

}

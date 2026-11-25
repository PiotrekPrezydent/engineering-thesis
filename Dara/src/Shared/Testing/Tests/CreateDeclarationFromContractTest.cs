using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using Dara.Shared.Contracts.Functions;
using Microsoft.Extensions.AI;

namespace Dara.Shared.Testing.Tests;

// declaration from the contract equals the one meai makes by reflection, without a model call
public class CreateDeclarationFromContractTest : ITest
{
    public Task Run(string api)
    {
        (Delegate method, FunctionDescription description)[] cases =
        [
            (Echo, EchoDescription),
            (Paint, PaintDescription),
            (FindPerson, FindPersonDescription)
        ];

        var matching = 0;
        foreach (var (method, description) in cases)
        {
            var declaration = AIFunctionFactory.CreateDeclaration(
                description.Name,
                description.Description,
                FunctionJsonSchema.CreateParametersSchema(description),
                FunctionJsonSchema.CreateReturnSchema(description));

            var reflected = AIFunctionFactory.Create(method);

            var sameParameters = JsonElement.DeepEquals(reflected.JsonSchema, declaration.JsonSchema);
            var sameReturn = SameOptional(reflected.ReturnJsonSchema, declaration.ReturnJsonSchema);

            Console.WriteLine($"## {declaration.Name}: {declaration.Description}");
            Console.WriteLine($"schema: {declaration.JsonSchema.GetRawText()}");
            Console.WriteLine($"return: {declaration.ReturnJsonSchema?.GetRawText() ?? "null"}");
            Console.WriteLine($"same as reflection: parameters {sameParameters}, return {sameReturn}");

            if (!sameParameters)
                Console.WriteLine($"reflection schema: {reflected.JsonSchema.GetRawText()}");
            if (!sameReturn)
                Console.WriteLine($"reflection return: {reflected.ReturnJsonSchema?.GetRawText() ?? "null"}");

            if (sameParameters && sameReturn)
                matching++;
        }

        Console.WriteLine($"{matching} of {cases.Length} declarations match reflection");
        return Task.CompletedTask;
    }

    private static bool SameOptional(JsonElement? first, JsonElement? second)
    {
        if (first is null || second is null)
            return first is null && second is null;

        return JsonElement.DeepEquals(first.Value, second.Value);
    }

    public enum Color
    {
        Red,
        Green,
        Blue
    }

    public class Address
    {
        public string Street { get; set; } = "";

        [Description("Postal code")]
        public string? Zip { get; set; }
    }

    public class Person
    {
        [Description("Full name")]
        public required string Name { get; set; }

        public int Age { get; set; }

        public Address? Home { get; set; }

        public List<Address> Others { get; set; } = [];
    }

    [Description("Repeats a text")]
    private static string Echo([Description("Text to repeat")] string text, int count = 5, string? suffix = null)
        => text;

    [Description("Paints with every kind of parameter")]
    private static void Paint(
        Color color,
        Color? second,
        List<int> sizes,
        [Range(1, 10)] int level,
        [StringLength(20, MinimumLength = 2)] string label,
        [RegularExpression("^[a-z]+$")] string code,
        [MinLength(1), MaxLength(5)] string[] tags,
        Guid id,
        DateTime at,
        [Base64String] string data,
        Dictionary<string, int> counts,
        bool flag = true)
    {
    }

    [Description("Finds a person like the filter")]
    private static Person FindPerson(Person filter) => filter;

    private static TypeDescription Text => new(TypeKind.String);

    private static TypeDescription Integer => new(TypeKind.Integer);

    private static readonly IReadOnlyList<string> Colors = ["Red", "Green", "Blue"];

    private static readonly FunctionDescription EchoDescription = new(
        nameof(Echo),
        "Repeats a text",
        [
            new("text", Text with { Description = "Text to repeat" }, true),
            new("count", Integer with { Default = "5" }, false),
            new("suffix", Text with { Nullable = true, Default = "null" }, false)
        ],
        Text);

    private static readonly FunctionDescription PaintDescription = new(
        nameof(Paint),
        "Paints with every kind of parameter",
        [
            new("color", Text with { Enum = Colors }, true),
            new("second", Text with { Nullable = true, Enum = Colors }, true),
            new("sizes", new TypeDescription(TypeKind.Array, Items: Integer), true),
            new("level", Integer with { Minimum = 1, Maximum = 10 }, true),
            new("label", Text with { MinLength = 2, MaxLength = 20 }, true),
            new("code", Text with { Pattern = "^[a-z]+$" }, true),
            new("tags", new TypeDescription(TypeKind.Array, Items: Text, MinItems: 1, MaxItems: 5), true),
            new("id", Text with { Format = "uuid" }, true),
            new("at", Text with { Format = "date-time" }, true),
            new("data", Text with { ContentEncoding = "base64" }, true),
            new("counts", new TypeDescription(TypeKind.Object, AdditionalProperties: Integer), true),
            new("flag", new TypeDescription(TypeKind.Boolean, Default: "true"), false)
        ],
        null);

    private static TypeDescription AddressType(bool nullable) => new(TypeKind.Object, Nullable: nullable, Properties:
    [
        new("street", Text, false),
        new("zip", Text with { Nullable = true, Description = "Postal code" }, false)
    ]);

    private static readonly TypeDescription PersonType = new(TypeKind.Object, Properties:
    [
        new("name", Text with { Description = "Full name" }, true),
        new("age", Integer, false),
        new("home", AddressType(true), false),
        new("others", new TypeDescription(TypeKind.Array, Items: AddressType(false)), false)
    ]);

    private static readonly FunctionDescription FindPersonDescription = new(
        nameof(FindPerson),
        "Finds a person like the filter",
        [new("filter", PersonType, true)],
        PersonType);
}

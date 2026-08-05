namespace Dara.Shared.SourceGenerators.Attributes;

public static class AttributeNames
{
    public const string AttributesNamespace = "Dara.Shared.Attributes";

    public const string GenerateBuilderAttributeName = "GenerateBuilderAttribute";

    public const string BuilderMethodNameAttributeName = "BuilderMethodNameAttribute";

    public const string ObsoleteMethodOnRepeatedTypeAttributeName = "ObsoleteMethodOnRepeatedTypeAttribute";
    public static string GetFullyQualifiedName(string className) => AttributesNamespace + "." + className;
}



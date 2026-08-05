namespace Dara.Shared.SourceGenerators.BuilderCollections;

public class BuilderCollectionNames
{
    public const string Usings = $"using System;\n" +
                                 $"using System.Collections.Generic;";

    public const string Namespace = "Dara.Shared.SourceGenerators.BuilderCollections";

    public const string ClassName = "BuilderCollection";
    public const string TypeIgnoringClassName = "TypeIgnoringBuilderCollection";

    public static string GetFormatableText(string name) => Namespace + "." + name + "<{0}>";

}
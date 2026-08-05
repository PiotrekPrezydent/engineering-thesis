namespace Dara.Shared.SourceGenerators.Common;

public record FileDeclaration(string FileName, string Text)
{
    public string FileNameWithSuffix => FileName + ".g.cs";
}


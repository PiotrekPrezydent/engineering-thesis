using System.Collections.Generic;

namespace Dara.Shared.SourceGenerators.Common;

public static class Types
{
    public static readonly string[] CollectionTypes = new[]
    {
        typeof(List<>).FullName,
        typeof(IReadOnlyList<>).FullName,
        typeof(IEnumerable<>).FullName,
    };
}
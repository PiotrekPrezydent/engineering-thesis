using System;

namespace Dara.Shared.SourceGenerators.BuilderClass.Models;

[Flags]
public enum GenericConstraints
{
    None = 0,
    New = 1 << 0,
    NotNull = 1 << 1,
    Class = 1 << 2,
    Unmanaged = 1 << 3,
    Struct = 1 << 4,
}

public static class GenericConstraintsExtensions
{
    public static string ConstraintAsString(this GenericConstraints constraint)
    {
        return constraint switch
        {
            GenericConstraints.None => string.Empty,
            GenericConstraints.New => "new()",
            GenericConstraints.NotNull => "notnull",
            GenericConstraints.Class => "class",
            GenericConstraints.Unmanaged => "unmanaged",
            GenericConstraints.Struct => "struct",
            _ => throw new ArgumentException($"unexpected constraint {constraint}")
        };
    }
}
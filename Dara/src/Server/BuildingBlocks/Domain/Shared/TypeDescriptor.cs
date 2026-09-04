using System.Collections.Immutable;
using System.ComponentModel;

namespace Dara.Server.BuildingBlocks.Domain.Shared;

public record TypeDescriptor(string Name)
{
    public bool IsNullable = false;
    public bool IsArray = false;
    public ImmutableArray<string> EnumValues = ImmutableArray<string>.Empty;
    public ImmutableArray<System.ComponentModel.TypeDescriptor> Properties = ImmutableArray<System.ComponentModel.TypeDescriptor>.Empty;
}
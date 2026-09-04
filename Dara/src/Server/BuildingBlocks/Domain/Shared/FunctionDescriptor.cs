using System.Collections.Immutable;

namespace Dara.Server.BuildingBlocks.Domain.Shared;

public record FunctionDescriptor(string Name, string Description = "")
{
    public ImmutableArray<FunctionParameterDescriptor> Parameters = ImmutableArray<FunctionParameterDescriptor>.Empty;
    public TypeDescriptor ReturnType = new TypeDescriptor("void");
}
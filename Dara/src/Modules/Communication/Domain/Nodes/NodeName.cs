using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.Communication.Domain.Nodes;

public class NodeName : ValueObject
{
    public string Value { get; }
    
    public NodeName(string value)
    {
        Value = value;
    }
}
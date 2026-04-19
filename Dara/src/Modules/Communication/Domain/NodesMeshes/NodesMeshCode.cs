using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.Communication.Domain.NodesMeshs;

public class NodesMeshCode : ValueObject
{
    public string Value { get; }

    public NodesMeshCode(string value)
    {
        Value = value;
    }
}
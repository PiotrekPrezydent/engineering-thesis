using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.Communication.Domain.NodesMeshs;

public class NodesMeshName : ValueObject
{
    public string Value { get; }

    public NodesMeshName(string value)
    {
        Value = value;
    }
}
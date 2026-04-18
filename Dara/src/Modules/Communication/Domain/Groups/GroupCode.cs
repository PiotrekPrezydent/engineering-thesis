using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.Communication.Domain.Groups;

public class GroupCode : ValueObject
{
    public string Value { get; }

    public GroupCode(string value)
    {
        Value = value;
    }
}
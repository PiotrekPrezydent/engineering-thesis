using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.Communication.Domain.Groups;

public class GroupName : ValueObject
{
    public string Value { get; }

    public GroupName(string value)
    {
        Value = value;
    }
}
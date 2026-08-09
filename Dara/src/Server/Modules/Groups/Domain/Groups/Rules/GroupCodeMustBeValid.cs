using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups.Rules;

public class GroupCodeMustBeValid : IBuisnessRule
{
    private readonly string _groupCode;
    private readonly string _providedCode;

    public GroupCodeMustBeValid(string groupCode, string providedCode)
    {
        _groupCode = groupCode;
        _providedCode = providedCode;
    }
    public string Message => "Group code must be valid";
    public bool IsBroken()
    { 
        return _providedCode != _groupCode;
    }
}
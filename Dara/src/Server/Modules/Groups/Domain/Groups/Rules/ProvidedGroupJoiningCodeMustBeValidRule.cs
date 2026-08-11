using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups.Rules;

public class ProvidedGroupJoiningCodeMustBeValidRule : IBuisnessRule
{
    private readonly string _providedCode;
    private readonly string _groupCode;
    
    public ProvidedGroupJoiningCodeMustBeValidRule(string providedCode, string groupCode)
    {
        _providedCode = providedCode;
        _groupCode = groupCode;
    }
    
    public string Message => "Group code must be valid";
    public bool IsBroken()
    { 
        return  _groupCode != _providedCode;
    }
}
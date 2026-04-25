using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.Modules.Communication.Domain.Clients.Rules;

public class ClientNameCannotBeEmptyRule : ValueObjectMemberRule<ClientName, string>
{
    public ClientNameCannotBeEmptyRule(string givenValue) : base(givenValue)
    {
    }

    public override bool IsBroken()
    {
        return _givenValue.IsWhiteSpace();
    }
}
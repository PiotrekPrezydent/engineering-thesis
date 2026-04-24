using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.Modules.Communication.Domain.Clients.Rules;

public class ClientAuthTokenCannotBeEmptyRule : ValueObjectMemberRule<ClientAuthToken, string>
{
    public ClientAuthTokenCannotBeEmptyRule(string givenValue) : base(givenValue)
    {
    }

    public override bool IsBroken()
    {
        return _givenValue.IsWhiteSpace();
    }
}
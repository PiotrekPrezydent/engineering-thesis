using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Node;

public class NodeAuthToken : ValueObject
{
    internal string Token { get; }
    
    public NodeAuthToken(string token)
    {
        Token = token;
    }
}
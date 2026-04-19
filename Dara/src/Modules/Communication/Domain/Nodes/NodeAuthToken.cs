using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.Communication.Domain.Nodes;

public class NodeAuthToken : ValueObject
{
    public string Token { get; }
    
    public NodeAuthToken(string token)
    {
        Token = token;
    }
}
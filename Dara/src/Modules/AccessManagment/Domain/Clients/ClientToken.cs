using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Clients;

//add validation
public class ClientToken : ValueObject
{
    public string Token { get; }
    
    public ClientToken(string token)
    {
        Token = token;
    }
}
using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Clients;

//add validation
public class ClientName : ValueObject
{
    public string Name { get; }
    
    public ClientName(string name)
    {
        Name = name;
    }
}
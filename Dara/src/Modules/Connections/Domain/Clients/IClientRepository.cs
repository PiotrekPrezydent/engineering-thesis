using Dara.BuildingBlocks.Domain.Models.Abstraction;

namespace Dara.Modules.Connections.Domain.Clients
{
    public interface IClientRepository : IRepository<Client,ClientId>
    {
    
    }
}
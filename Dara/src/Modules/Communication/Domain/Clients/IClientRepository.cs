using Dara.BuildingBlocks.Domain.Models.Abstraction;

namespace Dara.Modules.Communication.Domain.Clients;

public interface IClientRepository : IRepository<Client,ClientId>
{
    
}
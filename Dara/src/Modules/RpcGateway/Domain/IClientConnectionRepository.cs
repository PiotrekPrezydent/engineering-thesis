using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.RpcGateway.Domain;

public interface IClientConnectionRepository : IRepository<ClientConnection>
{
    public Task<ClientConnection> FindByIdAsync(Guid clientId);
    
    public Task<ClientConnection> FindByConnectionId(ConnectionId connectionId);
    
    public Task AddAsync(ClientConnection clientConnection);
    
    public Task RemoveAsync(ClientConnection clientConnection);
    
    public Task SaveAsync(ClientConnection clientConnection);
}
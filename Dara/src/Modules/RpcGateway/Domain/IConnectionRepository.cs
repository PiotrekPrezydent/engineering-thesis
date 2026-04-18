using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.RpcGateway.Domain;

public interface IConnectionRepository : IRepository<Connection>
{
    public Task<Connection> GetByGuidAsync(Guid guid);
    
    public Task<Connection> GetByConnectionIdAsync(ConnectionId connectionId);
    
    public Task<IEnumerable<Connection>> GetAllAsync();
    
    public Task<IEnumerable<Connection>> GetAllWithIpAsync(ConnectionIp connectionIp);
    
    public Task AddAsync(Connection connection);
    
    public Task RemoveAsync(Connection connection);
}
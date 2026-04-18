using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.RpcGateway.Domain;

public interface IConnectionRepository : IRepository<Connection>
{
    public Task<Connection> GetByConnectionIdAsync(ConnectionId connectionId);
    
    public Task<IEnumerable<Connection>> GetAllWithIpAsync(ConnectionIp connectionIp);
}
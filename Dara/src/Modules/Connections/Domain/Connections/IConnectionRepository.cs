using Dara.BuildingBlocks.Domain.Models.Abstraction;

namespace Dara.Modules.Connections.Domain.Connections
{
    public interface IConnectionRepository : IRepository<Connection, ConnectionId>
    {
    }
}
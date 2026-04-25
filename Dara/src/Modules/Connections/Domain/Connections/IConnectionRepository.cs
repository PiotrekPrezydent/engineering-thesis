using Dara.BuildingBlocks.Domain.Models.Abstraction;

namespace Dara.Modules.Communication.Domain.Connections
{
    public interface IConnectionRepository : IRepository<Connection, ConnectionId>
    {
    }
}
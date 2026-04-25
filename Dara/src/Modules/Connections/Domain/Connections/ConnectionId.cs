using Dara.BuildingBlocks.Domain.Models;

namespace Dara.Modules.Connections.Domain.Connections
{
    public class ConnectionId : IdOfType<string>
    {
        public ConnectionId(string value) : base(value)
        {
        }
    }
}
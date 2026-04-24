using Dara.BuildingBlocks.Domain.Models;

namespace Dara.Modules.Communication.Domain.Connections;

public class ConnectionId : IdOfType<string>
{
    public ConnectionId(string value) : base(value)
    {
    }
}
using Dara.BuildingBlocks.Domain.Models;

namespace Dara.Modules.Connections.Domain.Connections
{
    public class ConnectionIp : ValueObject
    {
        public string Value { get; }

        public ConnectionIp(string connectionId) : base()
        {
            Value = connectionId;
        }
    }
}
using Dara.BuildingBlocks.Domain.Models;

namespace Dara.Modules.Connections.Domain.Clients
{
    public class ClientId : IdOfType<Guid>
    {
        public ClientId(Guid value) : base(value)
        {
        }
    }
}
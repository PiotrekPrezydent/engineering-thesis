using Dara.BuildingBlocks.Domain.Models;

namespace Dara.Server.Modules.Identity.Domain.Clients;

public class ClientId : IdOfType<Guid>
{
    public ClientId(Guid value) : base(value)
    {
    }
}
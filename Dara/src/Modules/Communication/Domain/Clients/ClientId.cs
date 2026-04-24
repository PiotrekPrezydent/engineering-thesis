using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Domain.Models;

namespace Dara.Modules.Communication.Domain.Clients;

public class ClientId : IdOfType<Guid>
{
    public ClientId(Guid value) : base(value)
    {
    }
}
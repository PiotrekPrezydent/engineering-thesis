using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain;

public record UserId(Guid Value) : BaseEntityId(Value)
{
    
}
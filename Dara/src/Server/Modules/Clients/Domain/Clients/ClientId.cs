using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients;

public record ClientId(Guid Value) : BaseEntityId(Value);
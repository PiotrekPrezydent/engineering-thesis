using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients;

// given by the server when a client is created, global and never changed; other modules know a client of the hub by
// it, Authorization knows every client (the panel too) by it as a subject
public record ClientId(Guid Value) : BaseEntityId(Value);

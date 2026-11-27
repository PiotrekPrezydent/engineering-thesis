using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Users.Clients;

// given by the server when a client registers, global and never changed; other modules know the client by it
public record UserClientId(Guid Value) : BaseEntityId(Value);
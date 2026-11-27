using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.ClientSessions;

public record ClientSessionId(Guid Value) : BaseEntityId(Value);
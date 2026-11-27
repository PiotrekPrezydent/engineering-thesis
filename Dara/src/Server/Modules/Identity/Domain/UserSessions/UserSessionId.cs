using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.UserSessions;

public record UserSessionId(Guid Value) : BaseEntityId(Value);

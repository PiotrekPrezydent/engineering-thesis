using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.UserSessions;

public record RefreshTokenId(Guid Value) : BaseEntityId(Value);
using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.ClientSessions;

public record RefreshTokenId(Guid Value) : BaseEntityId(Value);
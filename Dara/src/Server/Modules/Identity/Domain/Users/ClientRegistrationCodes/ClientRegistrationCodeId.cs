using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Users.ClientRegistrationCodes;

public record ClientRegistrationCodeId(Guid Value) : BaseEntityId(Value);
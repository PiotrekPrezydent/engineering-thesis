using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Sessions;

// given by the server when the account panel creates the device, the same id in the devices module
public record DeviceId(Guid Value) : BaseEntityId(Value);

using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.RoleAssignments;

public record RoleAssignmentId(Guid Value) : BaseEntityId(Value);

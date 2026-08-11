using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public record GroupSnapshot(Guid GroupId, Guid OwnerId, string Name, string JoinCode, List<Guid> Members) : IEntitySnapshot;
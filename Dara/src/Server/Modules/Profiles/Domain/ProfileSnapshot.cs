using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Profiles.Domain;

public record ProfileSnapshot(Guid ProfileId, string Name) : IEntitySnapshot;
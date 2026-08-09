using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Groups.Application.GetValidGroup;

public record GetValidGroupQuery(string JoinCode) : IQuery<Guid?>;
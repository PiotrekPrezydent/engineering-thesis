using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Identity.Application.GetClient;

public record GetClientQuery(string ClientIdentifier) : IQuery<Guid?>;
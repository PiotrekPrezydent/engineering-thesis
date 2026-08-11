using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Identity.Application.GetUser;

public record GetUserQuery(string UserIdentifier) : IQuery<Guid?>;
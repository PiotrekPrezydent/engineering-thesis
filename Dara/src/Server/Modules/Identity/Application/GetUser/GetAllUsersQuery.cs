using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Identity.Application.GetUser;

public record GetAllUsersQuery() : IQuery<List<(Guid userId, string userIdentifier)>>;
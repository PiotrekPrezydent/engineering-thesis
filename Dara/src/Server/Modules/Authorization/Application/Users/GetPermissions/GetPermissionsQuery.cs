using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Authorization.Application.Users.GetPermissions;

// the caller asks about itself: the account (panel) without a client, a client with its id
public record GetPermissionsQuery(Guid UserId, Guid? ClientId) : IQuery<CallerPermissionsDto>;

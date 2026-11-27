using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Authorization.Application;

// the host checks a permission with CheckPermissionsQuery, like any other query of a module
public interface IAuthorizationModule : IModule
{
}

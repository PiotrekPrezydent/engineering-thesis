using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Authorization.Application;

// the host checks a permission with IsAuthorizedQuery (who calls, which permission, on which resource), like any
// other query of a module; no module calls it
public interface IAuthorizationModule : IModule
{
}

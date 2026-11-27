using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.Definitions.SynchronizeDefinitions;

// sent once at start-up, after every module is initialized, with the checked definitions of all modules; the only way
// permissions and roles change
public record SynchronizeDefinitionsCommand(IReadOnlyList<IModuleAuthorizationDefinition> Definitions) : ICommand;

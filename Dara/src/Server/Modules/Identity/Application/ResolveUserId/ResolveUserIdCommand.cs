using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.ResolveUserId;

public record ResolveUserIdCommand(string UserIdentifier) : ICommand<Guid>;
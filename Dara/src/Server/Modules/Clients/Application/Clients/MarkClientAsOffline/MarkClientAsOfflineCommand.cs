using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Clients.Application.Clients.MarkClientAsOffline;

public record MarkClientAsOfflineCommand(Guid ClientId) : ICommand;
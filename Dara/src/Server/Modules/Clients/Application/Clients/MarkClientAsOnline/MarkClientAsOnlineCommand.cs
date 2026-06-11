using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Clients.Application.Clients.MarkClientAsOnline;

public record MarkClientAsOnlineCommand(Guid ClientId) : ICommand;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.CreateClient;

public record CreateClientCommand(string ClientIdentifier) : ICommand<Guid>;
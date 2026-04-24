using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients.GetClient;

public abstract record GetClientCommand() : IApplicationCommand;

public record GetClientByConnectionIdCommand(string ConnectionId) : GetClientCommand;

public record GetClientByGuidCommand(Guid ClientId) : GetClientCommand;


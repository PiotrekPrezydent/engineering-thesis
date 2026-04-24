using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients.DeleteClient;

public abstract record DeleteClientCommand() : IApplicationCommand;

public record DeleteClientByConnectionIdCommand(string ConnectionId) : DeleteClientCommand;

public record DeleteClientByClientGuid(Guid ClientGuid) : DeleteClientCommand;

public record DeleteClientByNodeGuid(Guid NodeGuid) : DeleteClientCommand;

using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Clients.Application.Clients.RemoveClientSupervisor;

public record RemoveClientSupervisorCommand(Guid ClientId, Guid SupervisorId) : ICommand;
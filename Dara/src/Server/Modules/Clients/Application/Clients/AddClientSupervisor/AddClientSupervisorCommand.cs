using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Clients.Application.Clients.AddClientSupervisor;

public record AddClientSupervisorCommand(Guid ClientId, Guid SupervisorId) : ICommand;
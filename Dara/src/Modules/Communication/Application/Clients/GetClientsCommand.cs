using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Communication.Application.Clients;

public abstract record GetClientsCommand() : IApplicationCommand;

public record GetAllClientsCommand() : GetClientsCommand;

public record GetAllClientWithIpCommand(string ClientIp);

public record GetClientsCommandResult(IEnumerable<GetClientCommandResult> clients) : IApplicationCommandResult;

public class GetClientsCommandHandler : IApplicationCommandHandler<GetClientsCommand, GetClientsCommandResult>
{
    private readonly IClientsRepository _clientsRepository;
    public GetClientsCommandHandler(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<GetClientsCommandResult> HandleAsync(GetClientsCommand command)
    {
        if (command is GetAllClientsCommand)
        {
            var clients = await _clientsRepository.GetAllAsync();
            return new(clients.Select(e =>
                new GetClientCommandResult(e.Id, e.ClientConnectionId.Value, e.ClientConnectionIp.Value)));
        }


        return null;
    }
}
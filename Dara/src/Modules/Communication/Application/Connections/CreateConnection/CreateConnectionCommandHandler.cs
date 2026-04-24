using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Connections;

namespace Dara.Modules.Communication.Application.Connections.CreateConnection;

public class CreateConnectionCommandHandler : IApplicationCommandHandler<CreateConnectionCommand, CreateConnectionCommandResult>
{
    IConnectionRepository _connectionRepository;
    
    public CreateConnectionCommandHandler(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }
    
    public async Task<CreateConnectionCommandResult> HandleAsync(CreateConnectionCommand command)
    {
        ConnectionId id = new(command.ConnectionId);
        ConnectionIp ip = new(command.ConnectionIp);
        
        Connection connection = Connection.Create(id, ip);
        
        await _connectionRepository.AddAsync(connection);
        
        return new();
    }
}
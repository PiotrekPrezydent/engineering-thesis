using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Modules.Communication.Domain.Connections;

namespace Dara.Modules.Communication.Application.Connections.DeleteConnection;

public class DeleteConnectionCommandHandler : IApplicationCommandHandler<DeleteConnectionCommand, DeleteConnectionCommandResult>
{
    IConnectionRepository _connectionRepository;
    
    public DeleteConnectionCommandHandler(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }
    
    public async Task<DeleteConnectionCommandResult> HandleAsync(DeleteConnectionCommand command)
    {
        ConnectionId connectionId = new(command.ConnectionId);

        Connection connection = await _connectionRepository.GetByIdAsync(connectionId);
        
        connection.Delete();
        
        await _connectionRepository.RemoveAsync(connection);
        
        return new();
    }
}
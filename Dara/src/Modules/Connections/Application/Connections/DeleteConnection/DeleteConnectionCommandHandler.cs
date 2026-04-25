using Dara.BuildingBlocks.Application.Commands;
using Dara.Modules.Connections.Domain.Connections;

namespace Dara.Modules.Connections.Application.Connections.DeleteConnection
{
    public class DeleteConnectionCommandHandler : IModuleCommandHandler<DeleteConnectionCommand, DeleteConnectionCommandResult>
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
        
            connection.Delete(); //true deletion is in event handler
        
            await _connectionRepository.SaveAsync(connection);
        
            return new();
        }
    }
}
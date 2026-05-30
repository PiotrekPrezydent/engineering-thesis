using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Connections;

namespace Dara.Modules.Connections.Application.Connections.DeleteConnection
{
    public class DeleteConnectionCommandHandler : IHandler<DeleteConnectionCommand>
    {
        IConnectionRepository _connectionRepository;
    
        public DeleteConnectionCommandHandler(IConnectionRepository connectionRepository)
        {
            _connectionRepository = connectionRepository;
        }
    
        public async Task HandleAsync(DeleteConnectionCommand command)
        {
            ConnectionId connectionId = new(command.ConnectionId);

            Connection connection = await _connectionRepository.GetByIdAsync(connectionId);
        
            connection.Delete();
        
            await _connectionRepository.RemoveAsync(connection);
        }
    }
}
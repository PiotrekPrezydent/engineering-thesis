using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Connections;

namespace Dara.Modules.Connections.Application.Connections.CreateConnection
{
    public class CreateConnectionCommandHandler : IHandler<CreateConnectionCommand>
    {
        IConnectionRepository _connectionRepository;
    
        public CreateConnectionCommandHandler(IConnectionRepository connectionRepository)
        {
            _connectionRepository = connectionRepository;
        }
    
        public async Task HandleAsync(CreateConnectionCommand command)
        {
            ConnectionId id = new(command.ConnectionId);
            ConnectionIp ip = new(command.ConnectionIp);
        
            Connection connection = Connection.Create(id, ip);
        
            await _connectionRepository.AddAsync(connection);
        }
    }
}
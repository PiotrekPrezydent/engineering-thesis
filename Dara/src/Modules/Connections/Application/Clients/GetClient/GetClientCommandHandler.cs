using Dara.BuildingBlocks.Application;
using Dara.Modules.Connections.Domain.Clients;
using Dara.Modules.Connections.Domain.Connections;

namespace Dara.Modules.Connections.Application.Clients.GetClient
{
    public class GetClientCommandHandler : IHandler<GetClientCommand>
    {
        private IConnectionRepository _connectionRepository;
    
        public GetClientCommandHandler(IConnectionRepository connectionRepository)
        {
            _connectionRepository = connectionRepository;
        }
    
        public async Task HandleAsync(GetClientCommand command)
        {
            ConnectionId id = new(command.ConnectionId);

            Connection connection = await _connectionRepository.GetByIdAsync(id);
            if (connection.TryGetClient(out Client client))
            {
               
            }
            else
            {
                throw new InvalidOperationException($"Could not find client with connection id {command.ConnectionId}");
            }
        }
    }
}
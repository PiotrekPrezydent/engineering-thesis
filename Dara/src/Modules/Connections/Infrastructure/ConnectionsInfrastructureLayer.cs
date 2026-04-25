using Dara.BuildingBlocks.Infrastructure;

namespace Dara.Modules.Connections.Infrastructure;

public class ConnectionsInfrastructureLayer : IInfrastructureLayer
{
    public IReadOnlyList<Type> GetRepositoriesImplementations =>
    [
        typeof(ClientRepository),
        typeof(ConnectionRepository)
    ];
}
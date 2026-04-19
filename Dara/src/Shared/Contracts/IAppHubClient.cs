using Dara.Shared.Contracts.Clients;

namespace Dara.Shared.Contracts;

public interface IAppHubClient : IGuestClient, INodeClient, INodeMeshClient;
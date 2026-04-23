using Dara.Shared.Contracts.Interactions;

namespace Dara.Shared.Contracts;

public interface IAppHubClient : IGuestClient, INodeClient, INodeMeshClient;
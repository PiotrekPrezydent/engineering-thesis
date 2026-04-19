using Dara.Shared.Contracts.Communication;
using Dara.Shared.Contracts.Connection;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Shared.Contracts;

public interface IAppHub : IGuestInteractions, INodeInteractions, INodeMeshInteractions;
 
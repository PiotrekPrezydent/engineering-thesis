using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Node;

public class NodeIdentity : Entity
{
    internal Guid UserId { get; set; }
    
    internal string DeviceName  { get; private set; }
    
    internal NodeAuthToken AuthToken { get; private set; }

    internal NodeIdentity(Guid userId, string deviceName, NodeAuthToken authToken)
    {
        DeviceName = deviceName;
        AuthToken = authToken;
    }
}
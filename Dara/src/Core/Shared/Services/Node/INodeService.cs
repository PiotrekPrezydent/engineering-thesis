namespace Dara.Core.Shared.Services.Node;

public record RegisterNodeRequest(Guid UserId, string DeviceName) : IDaraServiceRequest;
public record RegisterNodeResponse(Guid DeviceId) : IDaraServiceResponse;

public record RevokeNodeRequest(Guid DeviceId) : IDaraServiceRequest;
public record RevokeNodeResponse() : IDaraServiceResponse;

public interface INodeService : IDaraService
{
    public Task<RegisterNodeResponse> RegisterNodeAsync(RegisterNodeRequest request);
    
    public Task<RevokeNodeResponse> RevokeNodeAsync(RevokeNodeRequest request);
}
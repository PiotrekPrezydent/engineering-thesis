using Dara.Shared.Contracts.Connection;

namespace Dara.Shared.Contracts.Communication;

public interface ICommunicationHub
{
    //client
    public Task<ResponseDto> TryBecomeClientAsync(RegisterClientRequest request);
    
    public Task<ResponseDto> TryExitClientAsync(LeaveClientRequest request);
    
    public Task<ResponseDto> TryChangeClientNameAsync(ChangeClientNameRequest request);
    
    public Task<ResponseDto> TryChangeClientAuthTokenAsync(ChangeClientAuthTokenRequest request);
    
    public Task SendMessageToClientAsync(ClientDto client, MessageDto message);
    
    //group
    public Task<ResponseDto> TryJoinGroupAsync(JoinGroupRequest request);
    
    public Task<ResponseDto> TryLeaveGroupAsync(LeaveGroupRequest request);
    
    public Task<ResponseDto> TryCreateGroupAsync(CreateGroupRequest request);
    
    public Task<ResponseDto> TryChangeGroupNameAsync(ChangeGroupNameRequest request);
    
    public Task<ResponseDto> TryChangeGroupCodeAsync(ChangeGroupCodeRequest request);

    //public Task SendMessageToGroupAsync(GroupDto group, MessageDto message); //only group members should be able to do this
}
using Dara.Shared.Contracts.Connection;

namespace Dara.Shared.Contracts.Communication;

public interface ICommunicationHubClient
{
    public Task ReceiveMessageFromClientAsync(ClientDto sender, MessageDto message);

    public Task ReceiveMessageFromGroupAsync(MessageDto message);

    public Task ReceiveAllGroups(string[] groupNames);
    
    public Task ReceiveGroupStateAsync(GroupDto group);
}
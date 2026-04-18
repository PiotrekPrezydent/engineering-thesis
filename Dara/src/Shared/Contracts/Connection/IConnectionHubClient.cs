namespace Dara.Shared.Contracts.Connection;

public interface IConnectionHubClient
{
    //Receive message from server
    public Task ReceiveMessageAsync(MessageDto message);
}
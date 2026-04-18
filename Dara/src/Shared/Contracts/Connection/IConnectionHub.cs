namespace Dara.Shared.Contracts.Connection;

public interface IConnectionHub
{
    //Broadcast message to all connections
    public Task BroadcastMessageAsync(MessageDto message);
    
    //Brodcast message to specific connections
    public Task BroadcastMessageAsync(MessageDto message, params string[] connectionIds);

    //Brodcast message to connections with given Ip
    public Task BrodcastMessageAsync(MessageDto message, string connectionIp);
}
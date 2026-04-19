namespace Dara.Shared.Contracts.Clients;

public interface IGuestClient
{
    //receive client connection IP
    public Task ReceiveConnectionIpAsync();

    //receive client connection ID
    public Task ReceiveConnectionIdAsync();

    //all clients from specific Ip;
    public Task ReceiveAllConnectionsFromIpAsync();
}
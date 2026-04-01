namespace LADR.Prototype.WebServer.shared;

public interface IHubClient
{
    Task ReceiveMessage(string message);
}
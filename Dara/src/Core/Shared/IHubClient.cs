namespace Dara.Core.Shared;

public interface IHubClient
{
    public Task GetMessage(string message);
    
    public Task GetExcetpion(Exception ex);
}
namespace Dara.Shared.Contracts;

public interface IAppClient
{
    public Task GetException(string exception);

    public Task GetMessage(string message);

    //public Task GetWorkspaceList();
    
}
namespace Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

//commits changes to module

public interface IUnitOfWork
{
    public Task<int> CommitAsync();
}
namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.Persistence;

//commits changes to module

public interface IUnitOfWork
{
    public Task<int> CommitAsync();
}
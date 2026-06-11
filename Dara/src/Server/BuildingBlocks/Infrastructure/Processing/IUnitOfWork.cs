namespace Dara.BuildingBlocks.Infrastructure.Processing;

//commits changes to module

public interface IUnitOfWork
{
    public Task<int> CommitAsync();
}
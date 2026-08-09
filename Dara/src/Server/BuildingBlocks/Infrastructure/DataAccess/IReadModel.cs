namespace Dara.Server.BuildingBlocks.Application.Queries;

public interface IReadModel
{
    public IQueryable<TEntity> Query<TEntity>() where TEntity : class;
}
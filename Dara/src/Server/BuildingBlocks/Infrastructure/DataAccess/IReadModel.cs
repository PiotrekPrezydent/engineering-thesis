namespace Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

public interface IReadModel
{
    public IQueryable<TEntity> Query<TEntity>() where TEntity : class;
}
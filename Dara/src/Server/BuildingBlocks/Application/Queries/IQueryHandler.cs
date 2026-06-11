namespace Dara.Server.BuildingBlocks.Application.Queries;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    public Task<TResult> HandleAsync(TQuery query);
}
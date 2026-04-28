using Dara.BuildingBlocks.Domain;

namespace Dara.BuildingBlocks.Application;

public interface IHandler<in THandleable> where THandleable : IHandleable
{
    public Task HandleAsync(THandleable request);
}

public interface IHandler<in THandleable, TResult> where THandleable : IHandleable<TResult> where TResult : class
{
    public Task<TResult> HandleAsync(THandleable request);
}
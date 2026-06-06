using Dara.BuildingBlocks.Domain;

namespace Dara.Server.BuildingBlocks.Application;

public interface IHandler;

public interface IHandler<in THandleable> : IHandler where THandleable : IHandleable 
{
    public Task HandleAsync(THandleable request);
}

public interface IHandler<in THandleable, TResult> : IHandler where THandleable : IHandleable<TResult> where TResult : class
{
    public Task<TResult> HandleAsync(THandleable request);
}
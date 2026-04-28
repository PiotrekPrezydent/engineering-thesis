using Dara.BuildingBlocks.Domain;

namespace Dara.BuildingBlocks.Application;

public interface IModuleCommand : IHandleable;

public interface IModuleCommand<out TResult> : IHandleable<TResult> where TResult : class;
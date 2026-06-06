using Dara.BuildingBlocks.Domain;

namespace Dara.Server.BuildingBlocks.Application;

public interface IModuleCommand : IHandleable;

public interface IModuleCommand<out TResult> : IModuleCommand, IHandleable<TResult> where TResult : class;
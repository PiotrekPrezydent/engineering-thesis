namespace Dara.BuildingBlocks.Domain;

public interface IHandleable;

public interface IHandleable<out TResult> : IHandleable where TResult : class;
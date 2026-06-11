namespace Dara.Server.BuildingBlocks.Application.Commands;

public interface ICommand;

public interface ICommand<out TResult> : ICommand;
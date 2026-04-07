namespace Dara.Core.Domain.Commands;

public interface IApplicationCommandDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command) where TCommand : IApplicationCommand;
}
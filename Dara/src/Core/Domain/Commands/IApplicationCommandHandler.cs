namespace Dara.Core.Domain.Commands;

public interface IApplicationCommandHandler<in TCommand> where TCommand : IApplicationCommand
{
    Task HandleAsync(TCommand command);
}
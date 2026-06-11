using Dara.BuildingBlocks.Infrastructure.Processing;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.BuildingBlocks.Infrastructure.Decorating;

public class CommandHandlerUnitOfWorkDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
{
    readonly IUnitOfWork _unitOfWork;
    readonly ICommandHandler<TCommand> _decorated;
    
    public CommandHandlerUnitOfWorkDecorator(IUnitOfWork unitOfWork, ICommandHandler<TCommand> decorated)
    {
        _unitOfWork = unitOfWork;
        _decorated = decorated;
    }
    
    public async Task HandleAsync(TCommand command)
    {
        await _decorated.HandleAsync(command);
        await _unitOfWork.CommitAsync();
    }
}
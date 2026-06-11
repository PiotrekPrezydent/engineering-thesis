using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing;

namespace Dara.Server.BuildingBlocks.Infrastructure.Decorating;

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
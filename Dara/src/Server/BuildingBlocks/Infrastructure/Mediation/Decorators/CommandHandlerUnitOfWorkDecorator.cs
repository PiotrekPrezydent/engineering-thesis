using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

namespace Dara.Server.BuildingBlocks.Infrastructure.Mediation.Decorators;

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

public class CommandHandlerUnitOfWorkDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    readonly IUnitOfWork _unitOfWork;
    readonly ICommandHandler<TCommand,TResult> _decorated;
    
    public CommandHandlerUnitOfWorkDecorator(IUnitOfWork unitOfWork, ICommandHandler<TCommand, TResult> decorated)
    {
        _unitOfWork = unitOfWork;
        _decorated = decorated;
    }
    
    public async Task<TResult> HandleAsync(TCommand command)
    {
        var result = await _decorated.HandleAsync(command);
        await _unitOfWork.CommitAsync();
        return result;
    }
}
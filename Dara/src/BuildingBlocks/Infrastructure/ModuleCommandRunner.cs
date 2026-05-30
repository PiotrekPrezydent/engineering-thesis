using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.BuildingBlocks.Infrastructure.Configuration;
using Dara.Shared.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class ModuleCommandRunner : IModuleCommandRunner
{
    readonly protected IServiceProvider _serviceProvider;
    readonly BuildingBlocksLogger _logger;
    
    public ModuleCommandRunner(IServiceProvider serviceProvider, BuildingBlocksLogger logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<WrappedResult> ExecuteAsync<TCommand>(TCommand command) where TCommand : IModuleCommand
    {
        var handler = _serviceProvider.GetRequiredService<IHandler<TCommand>>();
        WrappedResult wrappedResult;
        
        _logger.ModuleCommandHandlerCalled(handler, command);

        try
        {
            await handler.HandleAsync(command);
            wrappedResult = new();
        }
        catch(Exception ex)
        {
            _logger.ModuleCommandHandlerException(handler, command, ex);
            wrappedResult = ex;
        }
        return wrappedResult;
    }

    public async Task<WrappedResult<TResult>> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : IModuleCommand<TResult> where TResult : class
    {
        var handler = _serviceProvider.GetRequiredService<IHandler<TCommand, TResult>>();
        WrappedResult<TResult> wrappedResult;
        
        _logger.ModuleCommandHandlerCalled(handler, command);
        
        try
        {
            var result = await handler.HandleAsync(command);
            _logger.ModuleCommandHandlerResult(handler, command, result);
            wrappedResult = result;
        }
        catch(Exception ex)
        {
            _logger.ModuleCommandHandlerException(handler, command, ex);
            wrappedResult = ex;
        }
        
        return wrappedResult;
    }
}
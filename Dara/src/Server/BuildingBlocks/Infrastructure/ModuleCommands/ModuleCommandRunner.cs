using Dara.Server.BuildingBlocks.Application;
using Dara.Shared.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dara.BuildingBlocks.Infrastructure.ModuleCommands;

public class ModuleCommandRunner : IModuleCommandRunner
{
    readonly protected IServiceProvider _serviceProvider;
    readonly protected ILogger<ModuleCommandRunner> _logger;
    
    public ModuleCommandRunner(IServiceProvider serviceProvider, ILogger<ModuleCommandRunner> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<WrappedResult> ExecuteAsync<TCommand>(TCommand command) where TCommand : IModuleCommand
    {
        WrappedResult wrappedResult;
        
        try
        {
            var handler = _serviceProvider.GetRequiredService<IHandler<TCommand>>();
            
            try
            {
                var task = handler.HandleAsync(command);
                Logging.ModuleCommandRunnerLogMessages.LogCommandHandlerCalled(_logger, handler.GetType().Name, command.GetType().Name, command);
                
                await task;
                wrappedResult = new();
            }
            catch (Exception handlerException)
            {
                Logging.ModuleCommandRunnerLogMessages.LogCommandHandlerException(_logger, handler.GetType().Name, command.GetType().Name, command, handlerException.GetType().Name, handlerException.Message);
                wrappedResult = handlerException;
            }
        }
        catch (Exception runnerException)
        {
            Logging.ModuleCommandRunnerLogMessages.LogModuleCommandRunnerException(_logger, runnerException.GetType().Name, runnerException.Message);
            wrappedResult = runnerException;
        }
        
        return wrappedResult;
    }

    public async Task<WrappedResult<TResult>> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : IModuleCommand<TResult> where TResult : class
    {
        WrappedResult<TResult> wrappedResult;

        try
        {
            var handler = _serviceProvider.GetRequiredService<IHandler<TCommand, TResult>>();
            
            try
            {
                var task = handler.HandleAsync(command);
                Logging.ModuleCommandRunnerLogMessages.LogCommandHandlerCalled(_logger, handler.GetType().Name, command.GetType().Name, command);

                var result = await task;
                wrappedResult = result;
            }
            catch (Exception handlerException)
            {
                Logging.ModuleCommandRunnerLogMessages.LogCommandHandlerException(_logger, handler.GetType().Name, command.GetType().Name, command, handlerException.GetType().Name, handlerException.Message);
                wrappedResult = handlerException;
            }
        }
        catch (Exception runnerException)
        {
            Logging.ModuleCommandRunnerLogMessages.LogModuleCommandRunnerException(_logger, runnerException.GetType().Name, runnerException.Message);
            wrappedResult = runnerException;
        }
        
        return wrappedResult;
    }
}
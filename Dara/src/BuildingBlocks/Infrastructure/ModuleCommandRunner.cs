using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.Shared.Common;
using Dara.Shared.Common.CLI;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class ModuleCommandRunner : IModuleCommandRunner
{
    readonly protected IServiceProvider _serviceProvider;
    protected CLILogger _logger;
    
    public ModuleCommandRunner(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = new(this.GetType().Name, ConsoleColor.Red);
    }

    public async Task<WrappedResult> ExecuteAsync<TCommand>(TCommand command) where TCommand : IModuleCommand
    {
        var handler = _serviceProvider.GetRequiredService<IHandler<TCommand>>();
        WrappedResult wrappedResult;
        
        string log1 = $"Calling handler of type: {handler.GetType().Name} for request of type {command.GetType().Name}";
        string log2 = $"Request value: {command}";
        string log3;

        try
        {
            await handler.HandleAsync(command);
            
            log3 = $"Handled without exception.";
            wrappedResult = new();
        }
        catch(Exception ex)
        {
            log3 = $"Handled with exception of type: {ex.GetType().Name}.";
            wrappedResult = ex;
        }
        
        _logger.LogMany(log1, log2, log3);
        
        return wrappedResult;
    }

    public async Task<WrappedResult<TResult>> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : IModuleCommand<TResult> where TResult : class
    {
        var handler = _serviceProvider.GetRequiredService<IHandler<TCommand, TResult>>();
        WrappedResult<TResult> wrappedResult;
        
        string log1 = $"Calling handler of type: {handler.GetType().Name} for request of type {command.GetType().Name}";
        string log2 = $"Request value: {command}";
        string log3;
        
        try
        {
            var result = await handler.HandleAsync(command);
            
            log3 = $"Handled with result: {result}";
            wrappedResult = result;
        }
        catch(Exception ex)
        {
            log3 = $"Handled with exception of type: {ex.GetType().Name}.";
            wrappedResult = ex;
        }
        
        _logger.LogMany(log1, log2, log3);
        return wrappedResult;
    }
}
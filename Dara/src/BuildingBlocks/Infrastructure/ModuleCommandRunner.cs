using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.Shared.Common;
using Dara.Shared.Common.CLI;
using Dara.Shared.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class ModuleCommandRunner : IModuleCommandRunner
{
    readonly protected IServiceProvider _serviceProvider;
    protected Logger _logger;
    
    public ModuleCommandRunner(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = new(nameof(ModuleCommandRunner),this, LoggingType.Console);
    }

    public async Task<WrappedResult> ExecuteAsync<TCommand>(TCommand command) where TCommand : IModuleCommand
    {
        var handler = _serviceProvider.GetRequiredService<IHandler<TCommand>>();
        WrappedResult wrappedResult;

        try
        {
            await handler.HandleAsync(command);
            wrappedResult = new();
        }
        catch(Exception ex)
        {
            wrappedResult = ex;
        }
        return wrappedResult;
    }

    public async Task<WrappedResult<TResult>> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : IModuleCommand<TResult> where TResult : class
    {
        var handler = _serviceProvider.GetRequiredService<IHandler<TCommand, TResult>>();
        WrappedResult<TResult> wrappedResult;
        
        try
        {
            var result = await handler.HandleAsync(command);
            wrappedResult = result;
        }
        catch(Exception ex)
        {
            wrappedResult = ex;
        }
        
        return wrappedResult;
    }
}
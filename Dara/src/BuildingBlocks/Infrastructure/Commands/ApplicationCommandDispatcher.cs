using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Abstraction;
using Dara.Shared.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure.Commands;

public class ApplicationCommandDispatcher : IApplicationCommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public ApplicationCommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<CommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command) 
        where TCommandResult : IApplicationCommandResult 
        where TCommand : IApplicationCommand
    {
        var handler = _serviceProvider.GetRequiredService<IApplicationCommandHandler<TCommand, TCommandResult>>();
        
        Guid id = Guid.NewGuid();
        Console.WriteLine($"\n\n{id} - Called command Handler: {handler.GetType().Name} with command: {typeof(TCommand).Name}");
        Console.WriteLine($"\n{id} - Command value: {command}");
        
        CommandResult cr = new();
        try
        {
            var result = await handler.HandleAsync(command);
            
            Console.WriteLine($"\n{id} - Result value: {result}");
            
            cr.SetExpectedResult(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n{id} - Got Exception: {ex.GetType()} : {ex.Message}");
            cr.SetException(ex);
        }
        
        return cr;
    }
}
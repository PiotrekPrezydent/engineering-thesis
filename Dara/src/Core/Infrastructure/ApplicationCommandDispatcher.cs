using Dara.Core.Domain.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Core.Infrastructure;

public class ApplicationCommandDispatcher : IApplicationCommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public ApplicationCommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : IApplicationCommand
    {
        var handlers = _serviceProvider.GetServices<IApplicationCommandHandler<TCommand>>();
        foreach (var handler in handlers)
            await handler.HandleAsync((dynamic) command);
    }
}
using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Decorating;
using Dara.Server.Modules.Clients.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.Modules.Clients.Infrastructure;

public class ClientsCompositionRoot : BaseCompositionRoot<ClientsCompositionRoot>
{
    protected override Type ApplicationType => typeof(IClientsModule);

    protected override IReadOnlyList<Type> GlobalDecorators =>
    [
        typeof(CommandHandlerUnitOfWorkDecorator<>)
    ];
    
    protected override IServiceCollection AddModuleContext(IServiceCollection services)
    {
        services.AddDbContext<ClientsContext>(options =>
        {
            options.UseInMemoryDatabase(typeof(ClientsContext).Name);
        });
        services.AddScoped<ModuleContext>(sp => sp.GetRequiredService<ClientsContext>());
        
        return services;
    }
}
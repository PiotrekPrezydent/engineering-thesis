using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.References;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;

public class ModuleDataAccessVisitor : IVisitor<ModuleDataAccessConfiguration>
{
    private readonly ModuleReferencesConfiguration _referencesConfiguration;
    private readonly IServiceCollection _serviceCollection;
    public ModuleDataAccessVisitor(ModuleReferencesConfiguration referencesConfiguration, IServiceCollection collection)
    {
        _referencesConfiguration = referencesConfiguration;
        _serviceCollection = collection;
    }
    
    public void Visit(ModuleDataAccessConfiguration instance)
    {
        _serviceCollection.AddScoped(typeof(IUnitOfWork), instance.UnitOfWork.Value);
        
        instance.ModuleContext.ExecuteGenericAction(new ModuleContextRegistrator(_serviceCollection));
        
        var repositories = _referencesConfiguration.InfrastructureAssembly.GetTypes().Where(e=>typeof(IRepository).IsAssignableFrom(e)).ToList();
        foreach (var repository in repositories)
        {
            var implementedInterface = repository.GetInterfaces().First();
            if(implementedInterface == typeof(IOutboxRepository) || implementedInterface == typeof(IInboxRepository))
                continue;
            
            _serviceCollection.AddScoped(implementedInterface,repository);
        }
    }
    
    public class ModuleContextRegistrator(IServiceCollection services) : IKeyedTypeAction<DbContext>
    {
        public void Execute<TType>(ITypeKey<DbContext> typeKey) where TType : DbContext
        {
            services.AddDbContext<TType>(options =>
            {
                options.UseInMemoryDatabase(typeof(TType).Name);
            });
            
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<TType>());
        }
    }
}
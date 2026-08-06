using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.DataAccess;

public class ModuleDataAccessVisitor : IVisitor<ModuleDataAccessDescriptor>
{
    private IServiceCollection _serviceCollection;
    public ModuleDataAccessVisitor(IServiceCollection collection)
    {
        _serviceCollection = collection;
    }
    
    public void Visit(ModuleDataAccessDescriptor instance)
    {
        instance.ModuleContext.ExecuteGenericAction(new ModuleContextRegistrator(_serviceCollection));
    }
    
    public class ModuleContextRegistrator(IServiceCollection services) : IKeyedTypeAction<ModuleContext>
    {
        public void Execute<TType>(ITypeKey<ModuleContext> typeKey) where TType : ModuleContext
        {
            services.AddDbContext<TType>(options =>
            {
                options.UseInMemoryDatabase(typeof(TType).Name);
            });
            
            services.AddScoped<ModuleContext>(sp => sp.GetRequiredService<TType>());
        }
    }
}
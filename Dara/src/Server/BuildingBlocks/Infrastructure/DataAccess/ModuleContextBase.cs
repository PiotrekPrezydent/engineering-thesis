using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

public class ModuleContextBase : DbContext, IReadModel
{
    public DbSet<InboxMessage> InboxMessages { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public ModuleContextBase(DbContextOptions options) : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        
        var name = GetType().Assembly.GetName().Name!;
        var dotIndex = name.LastIndexOf('.');
        var domainAssembly = AppDomain.CurrentDomain.Load(name.Substring(0, dotIndex+1)+"Domain");
        
        var typedIdTypes = domainAssembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(BaseEntityId)) && !t.IsAbstract);
        foreach (var type in typedIdTypes)
        {
            var converterType = typeof(StronglyTypedIdConverter<>).MakeGenericType(type);
            configurationBuilder.Properties(type).HaveConversion(converterType);
        }
   
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InboxMessageEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public IQueryable<TEntity> Query<TEntity>() where TEntity : class
    {
        return Set<TEntity>().AsNoTracking();
    }
}

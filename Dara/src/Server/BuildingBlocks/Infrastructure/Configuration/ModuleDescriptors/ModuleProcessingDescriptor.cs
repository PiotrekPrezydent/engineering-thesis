using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Persistence;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Scopes;
using Dara.Shared.Attributes;


namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleDescriptors;

[GenerateBuilder]
public partial class ModuleProcessingDescriptor : IVisitable<ModuleProcessingDescriptor>
{
    [ObsoleteMethodOnRepeatedType(typeof(IDomainEventsDispatcher))]
    public ITypeKey<IDomainEventsDispatcher> DomainEventDispatcher{ get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(ICommandExecutor))]
    public ITypeKey<ICommandExecutor> CommandExecutor { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IHandlersResolver))]
    public ITypeKey<IHandlersResolver> HandlersResolver { get; set; }

    [ObsoleteMethodOnRepeatedType(typeof(IUnitOfWork))]
    public ITypeKey<IUnitOfWork> UnitOfWork { get; set; }
    
    public IReadOnlyList<Type> MediationOpenTypes { get; set; }
    
    public void Accept(IVisitor<ModuleProcessingDescriptor> visitor)
    {
        visitor.Visit(this);
    }
}
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration;
using Dara.Shared.Attributes;


namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.ModuleDescriptors;

[GenerateBuilder]
public partial class ModuleEventsDescriptor : IVisitable<ModuleEventsDescriptor>
{
    public IReadOnlyList<ITypeKey<IIntegrationEvent>> Events { get; set; }
    
    [ObsoleteMethodOnRepeatedType(typeof(IEventBus))]
    public ITypeKey<IEventBus> EventBus { get; set; }
    

    public void Accept(IVisitor<ModuleEventsDescriptor> visitor)
    {
        visitor.Visit(this);
    }
}


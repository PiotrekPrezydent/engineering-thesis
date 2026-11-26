using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.DomainEvents;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;
using Dara.Shared.Attributes;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.Processing;

[GenerateBuilder]
public partial class ModuleProcessingConfiguration : IVisitable<ModuleProcessingConfiguration>
{
    [ObsoleteMethodOnRepeatedType(typeof(IDomainEventsDispatcher))]
    public ITypeKey<IDomainEventsDispatcher> DomainEventDispatcher{ get; set; }

    [ObsoleteMethodOnRepeatedType(typeof(ICommandExecutor))]
    public ITypeKey<ICommandExecutor> CommandExecutor { get; set; }

    [ObsoleteMethodOnRepeatedType(typeof(IInternalCommandScheduler))]
    public ITypeKey<IInternalCommandScheduler> InternalCommandScheduler { get; set; }

    [ObsoleteMethodOnRepeatedType(typeof(IInternalCommandProcessor))]
    public ITypeKey<IInternalCommandProcessor> InternalCommandProcessor { get; set; }

    public void Accept(IVisitor<ModuleProcessingConfiguration> visitor)
    {
        visitor.Visit(this);
    }
}

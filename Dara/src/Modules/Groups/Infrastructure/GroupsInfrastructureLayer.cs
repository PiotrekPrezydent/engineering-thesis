using Dara.BuildingBlocks.Infrastructure;

namespace Dara.Modules.Groups.Infrastructure;

public class GroupsInfrastructureLayer : IInfrastructureLayer
{
    public IReadOnlyList<Type> GetRepositoriesImplementations => [];
}
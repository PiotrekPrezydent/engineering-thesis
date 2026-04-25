namespace Dara.BuildingBlocks.Infrastructure;

public interface IInfrastructureLayer
{
    public IReadOnlyList<Type> GetRepositoriesImplementations { get; }
}
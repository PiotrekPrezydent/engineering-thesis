namespace Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;


public interface IVisitor<in T> where T : class
{
    public void Visit(T instance);
}
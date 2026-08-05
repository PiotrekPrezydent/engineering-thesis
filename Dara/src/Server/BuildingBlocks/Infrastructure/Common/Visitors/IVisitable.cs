namespace Dara.Server.BuildingBlocks.Infrastructure.Common.Visitors;

public interface IVisitable<out T> where T : class
{
    void Accept(IVisitor<T> visitor);
}

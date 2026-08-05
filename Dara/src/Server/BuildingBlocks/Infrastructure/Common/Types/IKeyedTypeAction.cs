namespace Dara.Server.BuildingBlocks.Infrastructure.Common.Types;

public interface IKeyedTypeAction<in T> where T : class
{
    void Execute<TType>(ITypeKey<T> typeKey) where TType : T;
}
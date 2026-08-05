using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Dara.Server.BuildingBlocks.Infrastructure.Common.Types;

//represent specific type closed in generic parameter
public interface ITypeKey<out T> where T : class
{
    
    public static ITypeKey<T> Instance { get; } = new TypeKey<T>();
    public Type Value => typeof(T);
    
    public void ExecuteGenericAction(IKeyedTypeAction<T> action)
    {
        
        action.Execute<T>(this);
    }
}
public class TypeKey<T> : ITypeKey<T> where T : class;




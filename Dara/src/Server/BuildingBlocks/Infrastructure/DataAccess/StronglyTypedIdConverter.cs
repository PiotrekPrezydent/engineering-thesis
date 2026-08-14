using System.Linq.Expressions;
using Dara.Server.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

public class StronglyTypedIdConverter<TId> : ValueConverter<TId, Guid>
    where TId : BaseEntityId
{
    public StronglyTypedIdConverter()
        : base(
            id => id.Value,
            CreateFactory())
    {
    }
    
    private static Expression<Func<Guid, TId>> CreateFactory()
    {
        var parameter = Expression.Parameter(typeof(Guid), "value");
        var constructor = typeof(TId).GetConstructor(new[] { typeof(Guid) });
        
        if (constructor == null)
            throw new InvalidOperationException($"wrong number of parameters for {typeof(TId)}");

        var newExpression = Expression.New(constructor, parameter);
        return Expression.Lambda<Func<Guid, TId>>(newExpression, parameter);
    }
}
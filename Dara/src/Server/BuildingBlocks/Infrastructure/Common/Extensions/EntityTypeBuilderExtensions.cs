using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Dara.Server.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;

public static class EntityTypeBuilderExtensions
{
    extension<T>(T) where T : Entity
    {
        public static string DbTableName => typeof(T).Name + "s";
    }
    
    public static EntityTypeBuilder<TEntity> ResolvePrivateFields<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var fields = typeof(TEntity).GetFields(bindingFlags);
        foreach (var field in fields)
        {
            if(field.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false))
                continue;
            
            var type = field.FieldType;
            var name = field.Name;
            if (type.IsPrimitive || type == typeof(string))
            {
                builder.Property(type, name).HasColumnName(name.FirstLetterUpperCase());
            }
            else
            {
                //td figure out what to do
            }
            
        }
        return builder;
    }
    
    public static string FirstLetterUpperCase(this string s)
    {
        Regex rgx = new Regex("[^a-zA-Z0-9]");
        s = rgx.Replace(s, "");
        if (s.Length > 0)
            return char.ToUpper(s[0]) + s.Substring(1);
        
        return s;
    }
}
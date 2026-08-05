using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dara.Shared.SourceGenerators.Extensions;

public static class EnumExtensions
{
    public static string FlagsValuesWithType<T>(this T enumValue) where T : Enum
    {
        bool hasFlagsAttribute = typeof(T).IsDefined(typeof(FlagsAttribute), false);
        var type = typeof(T);
        var value = type + "." + enumValue.ToString();
        if (hasFlagsAttribute)
        {
            var flags = Enum.GetValues(type).Cast<T>().Where(f => enumValue.HasFlag(f));
            value = string.Join(" | ", flags.Select(f => type + "." + f.ToString()));
        }

        return value;
    }

    public static T[] FlagsToArray<T>(this T enumValue) where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>().Where(e=>enumValue.HasFlag(e)).ToArray();
    }
    
    public static string GetVarianceModifier(this GenericParameterAttributes attributes)
    {
        if (attributes.HasFlag(GenericParameterAttributes.Covariant))
            return "out";
            
        if (attributes.HasFlag(GenericParameterAttributes.Contravariant))
            return "in";
            
        return string.Empty;
    }
    
    public static List<string> GetConstraints(this GenericParameterAttributes attributes)
    {
        var constraints = new List<string>();
        
        if (attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
        {
            constraints.Add("class");
        }
        else if (attributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
        {
            constraints.Add("struct");
        }
        
        // if (attributes.HasFlag(GenericParameterAttributes.AllowByRefLike))
        // {
        //     constraints.Add("allows ref struct");
        // }
        
        if (attributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) && 
            !attributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
        {
            constraints.Add("new()");
        }

        return constraints;
    }
}
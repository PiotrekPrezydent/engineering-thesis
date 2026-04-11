using System.ComponentModel;
using System.Reflection;
using Dara.Core.Shared.Exceptions;

namespace Dara.Core.Shared.Services;

public static class RequestFactory
{
    public static TRequest CreateFromArgs<TRequest>(params object[] providedArg) where TRequest : IDaraServiceRequest
    {
        var requestParams = typeof(TRequest).GetConstructors().Single().GetParameters();
        
        if (requestParams.Length != providedArg.Length)
            throw new InvalidNumberOfArgs(requestParams.Length, providedArg.Length);
        
        object[] constructorArgs = new object[providedArg.Length];
        for (int i = 0; i < providedArg.Length; i++)
        {
            var parameter = requestParams[i];
            var provided = providedArg[i];
            var providedType = provided.GetType();
            if (providedType == typeof(string))
            {
                if (ParseFromString(parameter, (string)provided, out var parsed))
                    constructorArgs[i] = parsed;
                else
                    throw new RequestParameterParseException(parameter, provided);
            }
            // else if (providedType == typeof())
            // {
            //     
            // }
            else
            {
                throw new ObjectTypeResolverNotFoundException(providedType);
            }
        }
        TRequest? request = (TRequest?)Activator.CreateInstance(typeof(TRequest), constructorArgs);
        //TD: Add more specific exception
        if (request == null)
        {
            throw new Exception();
        }

        return request;
    }

    static bool ParseFromString(ParameterInfo requestArg, string providedArg, out object parsedArg)
    {
        parsedArg = null;
        if (requestArg.ParameterType == typeof(string))
        {
            parsedArg = providedArg;
            return true;
        }
        
        Type targetType = Nullable.GetUnderlyingType(requestArg.ParameterType) ?? requestArg.ParameterType;
        
        //bool isNullable = targetType != requestArg.ParameterType;
        
        //if request arg in nullable we can skip it
        // if (isNullable && string.IsNullOrWhiteSpace(providedArg))
        //     parsedArg = "";

        if (targetType == typeof(Guid))
        {
            if (Guid.TryParse(providedArg, out Guid result))
            {
                parsedArg = result;
                return true;
            }
        }

        if (targetType.IsEnum)
            return Enum.TryParse(targetType, providedArg, ignoreCase: true, out parsedArg); 
        
        TypeConverter converter = TypeDescriptor.GetConverter(targetType);
 
        if (converter != null && converter.CanConvertFrom(typeof(string)))
        {
            try
            {
                if (converter.IsValid(providedArg))
                {
                    parsedArg = converter.ConvertFromInvariantString(providedArg);
                    return true;
                }
            }
            catch (Exception err)
            {
                return false;
            }
        }
        return false;
    }
}
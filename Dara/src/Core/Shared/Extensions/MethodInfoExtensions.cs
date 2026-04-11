using System.Reflection;

namespace Dara.Core.Shared.Extensions;

public static class MethodInfoExtensions
{
    public static bool CanBeCalledWithArgs(this MethodInfo method, params object[] args)
    {
        var parameters = method.GetParameters();
        if (parameters.Length != args.Length)
            return false;

        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].ParameterType != args[i].GetType())
                return false;
        }

        return true;
    }

    public static IEnumerable<KeyValuePair<Type, string>> MethodParametersAsDictionary(this MethodInfo method)
    {
        List<KeyValuePair<Type, string>> result = new();
        var parameters = method.GetParameters();
        
        foreach(var parameter in parameters)
        {
            result.Add(new(parameter.ParameterType, parameter.Name));
        }
        return result;
    }
}
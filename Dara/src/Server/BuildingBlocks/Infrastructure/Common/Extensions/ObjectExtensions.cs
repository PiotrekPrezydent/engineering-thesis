using System.Reflection;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;

namespace Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;

public static class ObjectExtensions
{
    extension<T>(T) where T : class
    {
        public static ITypeKey<T> AsTypeKey =>ITypeKey<T>.Instance;

        public static Assembly ContainingAssembly => typeof(T).Assembly;


    }
    
    private static readonly string[] _layers = { "Application", "Domain", "Infrastructure", "Integration" };
    public static string GetModuleName(this object obj)
    {
        var assemblyName = obj.GetType().Assembly.GetName().Name;
        if (assemblyName == null)
            return "ERROR NAMES WAS NULL";
        
        var names = assemblyName.Split('.');
        string moduleName = "NAME NOT FOUND";
        for (int i = names.Length - 1; i >= 0; i--)
        {
            if (!_layers.Contains(names[i]))
            {
                moduleName = names[i];
                break;
            }
        }
        return moduleName;
    }
    


    
}
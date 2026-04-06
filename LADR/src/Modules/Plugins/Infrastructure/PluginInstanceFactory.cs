using System.Reflection;
using LADR.Modules.Plugins.Domain;
using LADR.Modules.Plugins.Domain.Exceptions;
using LADR.Modules.Plugins.Domain.Models;

namespace LADR.Modules.Plugins.Infrastructure;

public class PluginInstanceFactory : IPluginInstanceFactory
{
    public PluginInstance CreateFromType(Type exportedPluginType)
    {
        object? instanceObj = Activator.CreateInstance(exportedPluginType);
        if (instanceObj == null)
            throw new PluginInstanceIsNullException();
                
        return new PluginInstance(instanceObj, exportedPluginType);
    }
    
    public PluginInstance CreateFromPath(string path)
    {
        Assembly assembly = Assembly.LoadFile(path);
        Type[] types = assembly.GetExportedTypes();

        if (types.Length == 0)
            throw new PluginExportedTypesIsEmptyException();

        if (types.Length > 1)
            throw new PluginHasManyExportedTypesException();
        
        return CreateFromType(types[0]);
    }

    public PluginInstance CreateFromJson(string json)
    {
        throw new NotImplementedException();
    }
}
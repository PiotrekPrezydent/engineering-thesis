using System.Reflection;
using LADR.Modules.Plugins.Domain;
using LADR.Modules.Plugins.Domain.Exceptions;
using LADR.Modules.Plugins.Domain.Models;

namespace LADR.Modules.Plugins.Infrastructure;

public class PluginFactory : IPluginFactory
{
    public Plugin CreateFromPluginInstance(PluginInstance instance)
    {
        var plugin = new Plugin(instance.ParentType.Name, GetPluginDescription(instance));
        
        List<PluginMethod> pluginMethods = new();
        MethodInfo[] typeMethods = instance.ParentType.GetMethods();
        
        foreach (var method in typeMethods)
        {
            List<MethodParameter> pluginMethodParameters = new();
            ParameterInfo[] methodParameters = method.GetParameters();

            foreach (var parameter in methodParameters)
            {
                if (parameter.Name == null)
                    throw new PluginMethodParameterNameIsNullException();
                
                var methodParameter = new MethodParameter(parameter.Name, GetParameterDescription(parameter), parameter.ParameterType);
                pluginMethodParameters.Add(methodParameter);
            }

            var pluginMethod = new PluginMethod(method.Name, GetMethodDescription(method), pluginMethodParameters);
            pluginMethods.Add(pluginMethod);
        }
        
        plugin.AddInstance(instance);
        plugin.AddMethodRange(pluginMethods);
        instance.AddPlugin(plugin);
        
        return plugin;
    }

    string GetPluginDescription(PluginInstance instance) => "WIP GETTING PLUGIN DESCRIPTION";
    
    string GetMethodDescription(MethodInfo method) => "WIP GETTING METHOD DESCRIPTION";
    
    string GetParameterDescription(ParameterInfo parameterInfo) => "WIP GETTING PARAMETER DESCRIPTION";
}
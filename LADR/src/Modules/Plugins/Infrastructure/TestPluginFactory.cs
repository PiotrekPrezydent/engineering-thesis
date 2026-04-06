using LADR.Modules.Plugins.Domain;
using LADR.Modules.Plugins.Domain.Models;

namespace LADR.Modules.Plugins.Infrastructure;

public class TestPluginFactory : IPluginFactory
{
    public Plugin CreateFromInstance(PluginInstance instance)
    {
        var p = new Plugin("TEST PLUGIN NAME", "TEST PLUGIN DESCRIPTION");
        p.AddInstance(instance);
        
        var methods = instance.ParentType.GetMethods();
        foreach (var method in methods)
        {
            List<MethodParameter> parameters = new();
            var methodInfoParameters = method.GetParameters();

            foreach (var parameter in methodInfoParameters)
            {
                var methodParameter = new MethodParameter(parameter.Name, "PARAMETER DESCRIPTION", parameter.ParameterType);
                parameters.Add(methodParameter);
            }

            var pluginMethod = new PluginMethod(method.Name, "PLUGIN METHOD DESCRIPTION", parameters);
            p.AddMethod(pluginMethod);
        }

        return p;
    }
}
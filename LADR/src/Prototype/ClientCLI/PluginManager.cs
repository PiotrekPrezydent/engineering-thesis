using System.Reflection;

namespace LADR.Prototype.ClientCLI;

public class PluginManager
{
    public List<object> Plugins;
    
    public PluginManager()
    {
        Plugins = new();
    }

    public void LoadPlugin(string path)
    {
        Assembly assembly = Assembly.LoadFrom(path);
        var type = assembly.GetExportedTypes().First();
        object instance =  Activator.CreateInstance(type);
        Plugins.Add(instance);
        
        Console.WriteLine($"Plugin loaded: {path}");
    }

    //1 plugin 1 exported type at this moment
    public void InvokePluginMethod(object instance, string methodName, params object[] args)
    {
        // var p1 = pm.Plugins.First();
        // pm.InvokePluginMethod(p1,"AlphaConsoleWriteLine","TESTMSG");

        
        MethodInfo method = instance.GetType().GetMethod(methodName);
        method.Invoke(instance, args);
    }
}
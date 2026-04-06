using LADR.Modules.Plugins.Application;

namespace LADR.Modules.Plugins.Tests.CLI;

class Program
{
    static void Main(string[] args)
    {
        ModuleInitializer.Initialize();

        var loader = ModuleInitializer.GetPluginLoaderCommands();
        var methods = ModuleInitializer.GetPluginMethodCommands();

        foreach (var p in loader.GetAllAvaiblePluginsPath(Directory.GetCurrentDirectory() + "/Plugins"))
        {
            loader.LoadPlugin(p);
        }
        
        
        methods.InvokePluginMethod("AlphaPlugin","AlphaConsoleWriteLine","JAKISTEKSTEST1234");
        
    }
}